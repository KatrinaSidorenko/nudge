namespace Nudge.Shared.Core.Localization;

// Central language contract shared by every layer (bot, domain, API). Telegram hands the bot a
// raw ISO-ish string (e.g. "en", "en-US", "ru"); everything past that boundary works with this
// enum instead of passing raw strings around.
public enum Language
{
    Unknown = 0,
    En,
    Ru,
}
