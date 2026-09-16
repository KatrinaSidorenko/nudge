namespace Nudge.Shared.Core.BusinessRulesEngine;

public interface IBusinessRule
{
    string Error { get; }

    bool IsMet();
}
