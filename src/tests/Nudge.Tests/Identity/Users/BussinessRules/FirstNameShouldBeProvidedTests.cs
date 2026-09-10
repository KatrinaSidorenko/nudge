using Nudge.Identity.Users.BussinessRules;
using Xunit;

namespace Nudge.Tests.Identity.Users.BussinessRules;

public class FirstNameShouldBeProvidedTests
{
    [Theory]
    [InlineData("Katerina")]
    [InlineData("K")]
    public void IsMet_WithNonBlankValue_ReturnsTrue(string firstName)
    {
        var rule = new FirstNameShouldBeProvided(firstName);

        Assert.True(rule.IsMet());
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void IsMet_WithBlankOrMissingValue_ReturnsFalse(string? firstName)
    {
        var rule = new FirstNameShouldBeProvided(firstName);

        Assert.False(rule.IsMet());
    }
}
