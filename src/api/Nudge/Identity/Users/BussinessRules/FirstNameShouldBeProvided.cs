using Nudge.Shared.Core.BusinessRulesEngine;

namespace Nudge.Identity.Users.BussinessRules;

// Telegram always sends first_name on the From object, so a missing/blank value means the
// caller isn't passing through a real Telegram profile.
public class FirstNameShouldBeProvided : IBusinessRule
{
    private readonly string? _firstName;

    public FirstNameShouldBeProvided(string? firstName) => _firstName = firstName;

    public string Error => "First name is required";

    public bool IsMet() => !string.IsNullOrWhiteSpace(_firstName);
}
