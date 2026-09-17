using Nudge.Learning.Decks.BussinessRules;
using Xunit;

namespace Nudge.UnitTests.Learning.Decks.BussinessRules;

public class DeckTitleShouldBeLessThanNCharactersTests
{
    [Fact]
    public void IsMet_When_TitleIsWithinLimit_Then_ReturnsTrue()
    {
        var rule = new DeckTitleShouldBeLessThanNCharacters("Inbox");

        Assert.True(rule.IsMet());
    }

    [Fact]
    public void IsMet_When_TitleExceedsLimit_Then_ReturnsFalse()
    {
        var rule = new DeckTitleShouldBeLessThanNCharacters(new string('a', DeckTitleShouldBeLessThanNCharacters.Length + 1));

        Assert.False(rule.IsMet());
    }

    [Fact]
    public void IsMet_When_TitleIsNull_Then_ReturnsFalse()
    {
        var rule = new DeckTitleShouldBeLessThanNCharacters(null!);

        Assert.False(rule.IsMet());
    }
}
