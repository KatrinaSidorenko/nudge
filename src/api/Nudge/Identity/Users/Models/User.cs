using Nudge.Identity.Users.BussinessRules;
using Nudge.Identity.Users.ValueObjects;
using Nudge.Shared.Core.BusinessRulesEngine;
using Nudge.Shared.Core.Model;

namespace Nudge.Identity.Users.Models;

public record User : Aggregate<UserId>
{
    public TelegramUserId TelegramUserId { get; private set; } = default!;
    public string FirstName { get; private set; } = default!;
    public string? LastName { get; private set; }
    public string? Username { get; private set; }
    public string? LanguageCode { get; private set; }

    public static User Create(UserId id, TelegramUserId telegramUserId, string firstName, string? lastName, string? username, string? languageCode)
    {
        BusinessRuleValidator.Validate(new TelegramUserIdShouldBePositive(telegramUserId));
        BusinessRuleValidator.Validate(new FirstNameShouldBeProvided(firstName));

        var user = new User
        {
            Id = id,
            TelegramUserId = telegramUserId,
            FirstName = firstName,
            LastName = lastName,
            Username = username,
            LanguageCode = languageCode,
        };

        // domain event?

        return user;
    }

    // /start creates-or-updates on every call (#29) — this covers the existing-user path,
    // overwriting the same four fields Create captures for a new user.
    public void UpdateProfile(string firstName, string? lastName, string? username, string? languageCode)
    {
        BusinessRuleValidator.Validate(new FirstNameShouldBeProvided(firstName));

        FirstName = firstName;
        LastName = lastName;
        Username = username;
        LanguageCode = languageCode;
    }
}
