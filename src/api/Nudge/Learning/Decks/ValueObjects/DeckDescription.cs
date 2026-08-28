namespace Nudge.Learning.Decks.ValueObjects;

public record DeckDescription
{
    public string? Value { get; }

    public DeckDescription(string? value)
    {
        Value = value;
    }

    public static implicit operator string?(DeckDescription description) => description.Value;
}
