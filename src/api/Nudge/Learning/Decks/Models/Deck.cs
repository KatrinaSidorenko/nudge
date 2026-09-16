using Nudge.Learning.Cards.Models;
using Nudge.Learning.Cards.ValueObjects;
using Nudge.Learning.Decks.BussinessRules;
using Nudge.Learning.Decks.ValueObjects;
using Nudge.Shared.Core.BusinessRulesEngine;
using Nudge.Shared.Core.Model;

namespace Nudge.Learning.Decks.Models;

public record Deck : Aggregate<DeckId>
{
    public DeckTitle Title { get; private set; } = default!;
    public DeckDescription? Description { get; private set; }
    public bool IsArchived { get; private set; }

    private readonly List<CardId> _cards =[];
    public IReadOnlyList<CardId> Cards => _cards.AsReadOnly();

    public static Deck Create(DeckId id, DeckTitle title, DeckDescription? description, bool isArchived = false)
    {
        BusinessRuleValidator.Validate(new DeckTitleShouldBeLessThanNCharacters(title));
        BusinessRuleValidator.Validate(new DeckDescriptionShouldBeLessThanNCharacters(description));

        var deck = new Deck
        {
            Id = id,
            Title = title,
            Description = description,
            IsArchived = isArchived,
        };

        // domain event?

        return deck;
    }
}
