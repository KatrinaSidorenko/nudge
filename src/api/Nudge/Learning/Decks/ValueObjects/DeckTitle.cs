namespace Nudge.Learning.Decks.ValueObjects;

public record DeckTitle
{
    public string Value { get; }

    public DeckTitle(string value)
    {
        Value = value;
    }

    public static implicit operator string(DeckTitle title) => title.Value;
}
