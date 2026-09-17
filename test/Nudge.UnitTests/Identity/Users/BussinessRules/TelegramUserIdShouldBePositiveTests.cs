using Nudge.Identity.Users.BussinessRules;
using Xunit;

namespace Nudge.UnitTests.Identity.Users.BussinessRules;

public class TelegramUserIdShouldBePositiveTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(long.MinValue)]
    public void IsMet_When_TelegramUserIdIsNotPositive_Then_ReturnsFalse(long telegramUserId)
    {
        var rule = new TelegramUserIdShouldBePositive(telegramUserId);

        Assert.False(rule.IsMet());
    }

    [Fact]
    public void IsMet_When_TelegramUserIdIsPositive_Then_ReturnsTrue()
    {
        var rule = new TelegramUserIdShouldBePositive(123456789L);

        Assert.True(rule.IsMet());
    }
}
