using Nudge.Identity.Users.BussinessRules;
using Xunit;

namespace Nudge.Tests.Identity.Users.BussinessRules;

public class TelegramUserIdShouldBePositiveTests
{
    [Theory]
    [InlineData(1L)]
    [InlineData(123456789L)]
    [InlineData(long.MaxValue)]
    public void IsMet_WithPositiveValue_ReturnsTrue(long telegramUserId)
    {
        var rule = new TelegramUserIdShouldBePositive(telegramUserId);

        Assert.True(rule.IsMet());
    }

    [Theory]
    [InlineData(0L)]
    [InlineData(-1L)]
    [InlineData(long.MinValue)]
    public void IsMet_WithNonPositiveValue_ReturnsFalse(long telegramUserId)
    {
        var rule = new TelegramUserIdShouldBePositive(telegramUserId);

        Assert.False(rule.IsMet());
    }
}
