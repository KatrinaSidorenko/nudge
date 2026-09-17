using Nudge.Learning.Decks.BussinessRules;
using Xunit;

namespace Nudge.UnitTests.Learning.Decks.BussinessRules;

public class DeckDescriptionShouldBeLessThanNCharactersTests
{
    [Fact]
    public void IsMet_When_DescriptionIsNull_Then_ReturnsTrue()
    {
        var rule = new DeckDescriptionShouldBeLessThanNCharacters(null);

        Assert.True(rule.IsMet());
    }

    [Fact]
    public void IsMet_When_DescriptionExceedsLimit_Then_ReturnsFalse()
    {
        var rule = new DeckDescriptionShouldBeLessThanNCharacters(new string('a', DeckDescriptionShouldBeLessThanNCharacters.Length + 1));

        Assert.False(rule.IsMet());
    }
}
