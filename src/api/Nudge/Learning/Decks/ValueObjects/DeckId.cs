namespace Nudge.Learning.Decks.ValueObjects;

public record DeckId
{
    public long Value { get; }

    public DeckId(long id)
    {
        Value = id;
    }

    public static DeckId Of(long id)
    {
        if (id == 0L)
        {
            throw new InvalidOperationException("Something went wrong with id");
        }

        return new DeckId(id);
    }

    public static implicit operator long(DeckId id) => id.Value;
}
