using Nudge.Bot.Localization;
using Nudge.Shared.Core.Localization;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Nudge.Bot.Commands;

// Registered like any other IBotCommandHandler, keyed on BotCommandType.Unknown, so
// TelegramPollingService's dispatch stays a single dictionary lookup with no special-casing —
// text that doesn't parse into a known command falls through to this handler.
public class UnknownCommandHandler(ITelegramBotClient botClient, IBotMessageResolver messageResolver) : BotCommandHandlerBase
{
    public override BotCommandType CommandType => BotCommandType.Unknown;

    public override async Task HandleAsync(Message message, CancellationToken cancellationToken)
    {
        var language = LanguageParser.Parse(message.From?.LanguageCode);
        var text = await messageResolver.GetUnknownCommandMessageAsync(language, cancellationToken);
        await botClient.SendMessage(message.Chat.Id, text, cancellationToken: cancellationToken);
    }
}
