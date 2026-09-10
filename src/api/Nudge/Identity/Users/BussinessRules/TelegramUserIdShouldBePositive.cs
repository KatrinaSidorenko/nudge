using Nudge.Shared.Core.BusinessRulesEngine;

namespace Nudge.Identity.Users.BussinessRules;

// Telegram user IDs are 64-bit integers assigned by Telegram and are always positive; a
// zero/negative value means the caller never went through real Telegram auth.
public class TelegramUserIdShouldBePositive : IBusinessRule
{
    private readonly long _telegramUserId;

    public TelegramUserIdShouldBePositive(long telegramUserId) => _telegramUserId = telegramUserId;

    public string Error => "Telegram user id must be a positive number";

    public bool IsMet() => _telegramUserId > 0;
}
