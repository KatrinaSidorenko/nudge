namespace Nudge.Learning.Decks.ValueObjects;

public record DeckDescription
{
    public string? Value { get; }

    public DeckDescription(string? value)
    {
        Value = value;
    }

    public static DeckDescription Of(string? description)
    {
        return new DeckDescription(description);
    }

    public static implicit operator string?(DeckDescription description) => description.Value;
}
