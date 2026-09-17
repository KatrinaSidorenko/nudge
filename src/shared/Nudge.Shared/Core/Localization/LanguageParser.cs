namespace Nudge.Shared.Core.Localization;

public static class LanguageParser
{
    public static Language Parse(string? rawLanguageCode)
    {
        if (string.IsNullOrWhiteSpace(rawLanguageCode))
        {
            return Language.Unknown;
        }

        // Telegram sends IETF tags like "en-US"; only the primary subtag matters here.
        var primaryTag = rawLanguageCode.Split('-', 2)[0];

        return primaryTag.ToLowerInvariant() switch
        {
            "en" => Language.En,
            "ru" => Language.Ru,
            _ => Language.Unknown,
        };
    }
}
