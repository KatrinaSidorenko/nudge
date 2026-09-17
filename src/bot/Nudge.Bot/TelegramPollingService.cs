using Nudge.Bot.Commands;
using Nudge.Bot.Localization;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace Nudge.Bot;

/// <summary>
/// Connects to the real Telegram Bot API via long polling, parses each incoming message into a
/// <see cref="BotCommandType"/> and dispatches it to the matching <see cref="IBotCommandHandler"/>.
/// The gRPC client is wired in later Phase 1 slices.
/// </summary>
public class TelegramPollingService(
    ITelegramBotClient botClient,
    IEnumerable<IBotCommandHandler> commandHandlers,
    IBotMessageResolver messageResolver,
    ILogger<TelegramPollingService> logger) : BackgroundService, IUpdateHandler
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var me = await botClient.GetMe(stoppingToken);
        logger.LogInformation("Nudge.Bot started long polling as @{BotUsername}", me.Username);

        await botClient.ReceiveAsync(this, receiverOptions: null, stoppingToken);
    }

    public async Task HandleUpdateAsync(ITelegramBotClient client, Update update, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Received update {UpdateId} of type {UpdateType} from chat {ChatId}",
            update.Id,
            update.Type,
            update.Message?.Chat.Id);

        var message = update.Message;
        if (message?.Text is null)
        {
            return;
        }

        var commandType = BotCommandParser.Parse(message.Text);
        if (commandType == BotCommandType.Unknown)
        {
            await SendLocalizedMessageAsync(message, messageResolver.GetUnknownCommandMessageAsync, cancellationToken);
            return;
        }

        var handler = commandHandlers.FirstOrDefault(h => h.CommandType == commandType);
        if (handler is null)
        {
            await SendLocalizedMessageAsync(message, messageResolver.GetUnknownCommandMessageAsync, cancellationToken);
            return;
        }

        try
        {
            await handler.HandleAsync(message, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error handling {CommandType} for chat {ChatId}", commandType, message.Chat.Id);
            await SendLocalizedMessageAsync(message, messageResolver.GetInternalErrorMessageAsync, cancellationToken);
        }
    }

    private async Task SendLocalizedMessageAsync(Message message, Func<string?, CancellationToken, Task<string>> resolveMessage, CancellationToken cancellationToken)
    {
        var text = await resolveMessage(message.From?.LanguageCode, cancellationToken);
        await botClient.SendMessage(message.Chat.Id, text, cancellationToken: cancellationToken);
    }

    public Task HandleErrorAsync(ITelegramBotClient client, Exception exception, HandleErrorSource source, CancellationToken cancellationToken)
    {
        // ApiRequestException carries the Telegram-side error code/description; anything else
        // (network blip, timeout) is logged but doesn't stop the receive loop — ReceiveAsync
        // retries internally.
        var message = exception is ApiRequestException apiEx
            ? $"Telegram API error [{apiEx.ErrorCode}]: {apiEx.Message}"
            : exception.ToString();

        logger.LogError(exception, "Polling error from {Source}: {Message}", source, message);
        return Task.CompletedTask;
    }
}
