using Nudge.Identity.Users.BussinessRules;
using Xunit;

namespace Nudge.UnitTests.Identity.Users.BussinessRules;

public class FirstNameShouldBeProvidedTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void IsMet_When_FirstNameIsMissing_Then_ReturnsFalse(string? firstName)
    {
        var rule = new FirstNameShouldBeProvided(firstName);

        Assert.False(rule.IsMet());
    }

    [Fact]
    public void IsMet_When_FirstNameIsProvided_Then_ReturnsTrue()
    {
        var rule = new FirstNameShouldBeProvided("Katerina");

        Assert.True(rule.IsMet());
    }
}
