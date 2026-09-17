using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Nudge.Identity.Data;
using Nudge.Identity.Users.Models;
using Nudge.Identity.Users.ValueObjects;
using Nudge.Shared.Core.BusinessRulesEngine;
using Nudge.Shared.Core.CQRS;
using Nudge.Shared.Core.Localization;
using Nudge.Shared.Core.Utils;

namespace Nudge.Identity.Users.Features.UpsertUser.V1;

public record UpsertUserCommand(long? TelegramId, string FirstName, string? LastName, string? Username, Language LanguageCode) : ICommand<UpsertUserResult>;
public record UpsertUserResult(long Id, bool IsNewUser);

public class UpsertUserHandler(ILogger<UpsertUserHandler> logger, IdentityDbContext dbContext, IIdGenerator<long> idGenerator) : ICommandHandler<UpsertUserCommand, UpsertUserResult>
{
    public async Task<UpsertUserResult> Handle(UpsertUserCommand request, CancellationToken cancellationToken)
    {
        // TelegramId is nullable on the wire to leave room for a future email/password login
        // path, but Telegram is the only supported identity today, so it's required for now.
        // The request DTO validator already enforces this; this is a defense-in-depth guard for
        // callers that dispatch the command directly.
        if (request.TelegramId is not { } telegramId)
        {
            throw new BusinessRuleValidationException("Telegram user id is required");
        }

        var languageCode = LanguageCode.Of(request.LanguageCode);

        var existingUser = await dbContext.Users
            .SingleOrDefaultAsync(u => u.TelegramUserId.Value == telegramId, cancellationToken);

        var isNewUser = existingUser is null;

        User user;
        if (existingUser is null)
        {
            user = User.Create(UserId.Of(idGenerator.CreateId()), TelegramUserId.Of(telegramId), request.FirstName, request.LastName, request.Username, languageCode);
            await dbContext.AddAsync(user, cancellationToken);
        }
        else
        {
            existingUser.UpdateProfile(request.FirstName, request.LastName, request.Username, languageCode);
            user = existingUser;
        }

        // User.Create/UpdateProfile already raise UserUpsertedEvent themselves — this is a rich
        // domain model, so the event belongs with the behavior that causes it, not here.
        return new UpsertUserResult(user.Id, isNewUser);
    }
}
