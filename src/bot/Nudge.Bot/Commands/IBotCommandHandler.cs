using Telegram.Bot.Types;

namespace Nudge.Bot.Commands;

public interface IBotCommandHandler
{
    BotCommandType CommandType { get; }

    Task HandleAsync(Message message, CancellationToken cancellationToken);
}
