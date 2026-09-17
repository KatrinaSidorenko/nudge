namespace Nudge.Bot.Commands;

// Parses a Telegram message's text into a BotCommandType. Only recognizes messages that look
// like commands ("/start", "/start@NudgeBot arg1 arg2"); anything else is Unknown.
public static class BotCommandParser
{
    public static BotCommandType Parse(string? messageText)
    {
        if (string.IsNullOrWhiteSpace(messageText) || messageText[0] != '/')
        {
            return BotCommandType.Unknown;
        }

        // "/start@NudgeBot" -> "start"; trailing arguments after the first space are ignored.
        var commandWord = messageText[1..].Split(' ', 2)[0].Split('@', 2)[0];

        return commandWord.ToLowerInvariant() switch
        {
            "start" => BotCommandType.Start,
            _ => BotCommandType.Unknown,
        };
    }
}
