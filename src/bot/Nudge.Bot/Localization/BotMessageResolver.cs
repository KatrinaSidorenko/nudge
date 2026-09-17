using Nudge.Shared.Core.Localization;

namespace Nudge.Bot.Localization;

// Messages live in code for now — only English is supported. language is accepted on every
// method (and ignored) so callers don't need to change once real localization/resource files
// land; today every language falls back to English.
public class BotMessageResolver : IBotMessageResolver
{
    public Task<string> GetWelcomeMessageAsync(string firstName, bool isReturningUser, Language language, CancellationToken cancellationToken)
        => Task.FromResult(isReturningUser
            ? $"👋 Welcome back, {firstName}! Ready to pick up where you left off?"
            : $"🎉 Welcome, {firstName}! I'm Nudge, your spaced-repetition study buddy.\n\n" +
              "🗂️ I'll help you build decks of flashcards and nudge you to review them right " +
              "when it counts, so what you learn actually sticks. 🧠✨\n\n" +
              "Let's get started!");

    public Task<string> GetUnknownCommandMessageAsync(Language language, CancellationToken cancellationToken)
        => Task.FromResult("🤔 Hmm, I don't know that command yet. Try /start to see what I can do!");

    public Task<string> GetInternalErrorMessageAsync(Language language, CancellationToken cancellationToken)
        => Task.FromResult("😅 Oops, something went wrong on my end. Please try again in a moment!");
}
