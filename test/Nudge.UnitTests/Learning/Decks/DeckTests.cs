using Nudge.Learning.Decks.BussinessRules;
using Nudge.Learning.Decks.Models;
using Nudge.Learning.Decks.ValueObjects;
using Nudge.Shared.Core.BusinessRulesEngine;
using Xunit;

namespace Nudge.UnitTests.Learning.Decks;

public class DeckTests
{
    [Fact]
    public void Create_When_InputIsValid_Then_ReturnsDeckWithMappedFields()
    {
        var id = DeckId.Of(1L);

        var deck = Deck.Create(id, DeckTitle.Of("Inbox"), DeckDescription.Of("Default deck"), isArchived: false);

        Assert.Equal(id, deck.Id);
        Assert.Equal("Inbox", deck.Title);
        Assert.Equal("Default deck", deck.Description);
        Assert.False(deck.IsArchived);
        Assert.Empty(deck.Cards);
    }

    [Fact]
    public void Create_When_TitleExceedsLimit_Then_ThrowsBusinessRuleValidationException()
    {
        var title = new string('a', DeckTitleShouldBeLessThanNCharacters.Length + 1);

        Assert.Throws<BusinessRuleValidationException>(() =>
            Deck.Create(DeckId.Of(1L), DeckTitle.Of(title), null, isArchived: false));
    }

    [Fact]
    public void Create_When_DescriptionExceedsLimit_Then_ThrowsBusinessRuleValidationException()
    {
        var description = new string('a', DeckDescriptionShouldBeLessThanNCharacters.Length + 1);

        Assert.Throws<BusinessRuleValidationException>(() =>
            Deck.Create(DeckId.Of(1L), DeckTitle.Of("Inbox"), DeckDescription.Of(description), isArchived: false));
    }
}
