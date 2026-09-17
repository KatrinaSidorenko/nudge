using Nudge.Shared.Core.Localization;

namespace Nudge.Bot.Users;

public interface IUserService
{
    Task<UpsertUserResult> UpsertUserAsync(long telegramId, string firstName, string? lastName, string? username, Language languageCode, CancellationToken cancellationToken);
}
