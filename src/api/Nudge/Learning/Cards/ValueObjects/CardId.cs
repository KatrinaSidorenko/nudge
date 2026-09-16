using Nudge.Learning.Decks.ValueObjects;

namespace Nudge.Learning.Cards.ValueObjects;

public class CardId
{
    public CardId(long id) => Value = id;

    public long Value { get; }

    public static implicit operator long(CardId id) => id.Value;

    public static DeckId Of(long id)
    {
        if (id == 0L)
        {
            throw new InvalidOperationException("Something went wrong with id");
        }

        return new DeckId(id);
    }
}
