using Telegram.Bot.Types;

namespace Nudge.Bot.Commands;

public abstract class BotCommandHandlerBase : IBotCommandHandler
{
    public abstract BotCommandType CommandType { get; }

    public abstract Task HandleAsync(Message message, CancellationToken cancellationToken);
}
