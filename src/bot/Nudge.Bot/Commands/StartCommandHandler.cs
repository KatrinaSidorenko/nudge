using Nudge.Bot.Localization;
using Nudge.Bot.Users;
using Nudge.Shared.Core.Localization;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Nudge.Bot.Commands;

// /start creates-or-updates the caller's profile on every call: a first-time user is created,
// a returning user has their profile (name/username/language) refreshed from their current
// Telegram data.
public class StartCommandHandler(ITelegramBotClient botClient, IUserService userService, IBotMessageResolver messageResolver) : BotCommandHandlerBase
{
    public override BotCommandType CommandType => BotCommandType.Start;

    public override async Task HandleAsync(Message message, CancellationToken cancellationToken)
    {
        var from = message.From ?? throw new InvalidOperationException("Received a /start message without a Telegram user.");

        // Telegram's raw language code is only known here; everything past this point uses the
        // shared Language contract instead of the raw string.
        var language = LanguageParser.Parse(from.LanguageCode);

        var result = await userService.UpsertUserAsync(from.Id, from.FirstName, from.LastName, from.Username, language, cancellationToken);

        var welcomeMessage = await messageResolver.GetWelcomeMessageAsync(from.FirstName, isReturningUser: !result.IsNewUser, language, cancellationToken);
        await botClient.SendMessage(message.Chat.Id, welcomeMessage, cancellationToken: cancellationToken);
    }
}
