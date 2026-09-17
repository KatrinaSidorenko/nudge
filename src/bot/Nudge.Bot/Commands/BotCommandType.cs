namespace Nudge.Bot.Commands;

// Bot-level command enum: every recognized Telegram slash command maps to exactly one of these,
// each with its own IBotCommandHandler. Grows as new commands land (CreateDeck, ...).
public enum BotCommandType
{
    Unknown = 0,
    Start,
}
