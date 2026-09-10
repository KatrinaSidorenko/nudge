using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace Nudge.Bot;

/// <summary>
/// Connects to the real Telegram Bot API via long polling and logs incoming updates.
/// Command handling and the gRPC client are wired in later Phase 1 slices.
/// </summary>
public class TelegramPollingService(
    ITelegramBotClient botClient,
    ILogger<TelegramPollingService> logger) : BackgroundService, IUpdateHandler
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var me = await botClient.GetMe(stoppingToken);
        logger.LogInformation("Nudge.Bot started long polling as @{BotUsername}", me.Username);

        await botClient.ReceiveAsync(this, receiverOptions: null, stoppingToken);
    }

    public Task HandleUpdateAsync(ITelegramBotClient client, Update update, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Received update {UpdateId} of type {UpdateType} from chat {ChatId}",
            update.Id,
            update.Type,
            update.Message?.Chat.Id);

        return Task.CompletedTask;
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
