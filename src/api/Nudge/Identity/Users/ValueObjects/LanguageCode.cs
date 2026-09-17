using Nudge.Shared.Core.Localization;

namespace Nudge.Identity.Users.ValueObjects;

public record LanguageCode
{
    public Language Value { get; }

    public LanguageCode(Language value)
    {
        Value = value;
    }

    public static LanguageCode Of(Language value) => new(value);

    public static implicit operator Language(LanguageCode languageCode) => languageCode.Value;
}
