using Nudge.Shared.Core.BusinessRulesEngine;

namespace Nudge.Learning.Decks.BussinessRules;

public class DeckTitleShouldBeLessThanNCharacters : IBusinessRule
{
    public const int Length = 256;

    private readonly string _title;

    public DeckTitleShouldBeLessThanNCharacters(string title) => _title = title;

    public string Error => $"Title is null or length is more than {Length} characters";

    public bool IsMet() => _title is not null && _title.Length <= Length;
}
