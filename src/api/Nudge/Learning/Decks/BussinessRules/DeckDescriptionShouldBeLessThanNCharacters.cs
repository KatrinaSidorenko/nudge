using Nudge.Shared.Core.BusinessRulesEngine;

namespace Nudge.Learning.Decks.BussinessRules;

public class DeckDescriptionShouldBeLessThanNCharacters : IBusinessRule
{
    public const int Length = 4096;
    private readonly string? _description;

    public DeckDescriptionShouldBeLessThanNCharacters(string? description) => _description = description;

    public string Error => $"Description is null or length is more than {Length} characters";

    public bool IsMet() => _description is null || _description.Length <= Length;
}
