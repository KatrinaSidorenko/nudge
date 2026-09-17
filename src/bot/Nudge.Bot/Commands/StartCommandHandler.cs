using Nudge.Bot.Localization;
using Nudge.Bot.NudgeApi;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Nudge.Bot.Commands;

// /start creates-or-updates the caller's profile on every call: a first-time user is created,
// a returning user has their profile (name/username/language) refreshed from their current
// Telegram data.
public class StartCommandHandler(ITelegramBotClient botClient, IUserApiClient userApiClient, IBotMessageResolver messageResolver) : BotCommandHandlerBase
{
    public override BotCommandType CommandType => BotCommandType.Start;

    public override async Task HandleAsync(Message message, CancellationToken cancellationToken)
    {
        var from = message.From ?? throw new InvalidOperationException("Received a /start message without a Telegram user.");

        var request = new UpsertUserRequest(from.Id, from.FirstName, from.LastName, from.Username, from.LanguageCode);
        await userApiClient.UpsertUserAsync(request, cancellationToken);

        var welcomeMessage = await messageResolver.GetWelcomeMessageAsync(from.FirstName, from.LanguageCode, cancellationToken);
        await botClient.SendMessage(message.Chat.Id, welcomeMessage, cancellationToken: cancellationToken);
    }
}
