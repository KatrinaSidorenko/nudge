using Nudge.Learning.Cards.ValueObjects;
using Nudge.Learning.Decks.Models;
using Nudge.Shared.Core.Model;

namespace Nudge.Learning.Cards.Models;

public record Card : Aggregate<CardId>
{
    private readonly List<Deck> _decks =[];
    public IReadOnlyList<Deck> Decks => _decks.AsReadOnly();

    public void AddToDeck(Deck deck)
    {
        _decks.Add(deck);
    }
}
