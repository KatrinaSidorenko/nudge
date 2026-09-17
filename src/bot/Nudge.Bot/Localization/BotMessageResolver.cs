using Nudge.Shared.Core.Localization;

namespace Nudge.Bot.Localization;

// Messages live in code for now — only English is supported. language is accepted on every
// method (and ignored) so callers don't need to change once real localization/resource files
// land; today every language falls back to English.
public class BotMessageResolver : IBotMessageResolver
{
    public Task<string> GetWelcomeMessageAsync(string firstName, bool isReturningUser, Language language, CancellationToken cancellationToken)
        => Task.FromResult(isReturningUser
            ? $"Welcome back, {firstName}!"
            : $"Welcome, {firstName}! I'm Nudge — I'll help you learn with spaced repetition flashcards.");

    public Task<string> GetUnknownCommandMessageAsync(Language language, CancellationToken cancellationToken)
        => Task.FromResult("Sorry, I don't recognize that command.");

    public Task<string> GetInternalErrorMessageAsync(Language language, CancellationToken cancellationToken)
        => Task.FromResult("Something went wrong on our side. Please try again in a moment.");
}
