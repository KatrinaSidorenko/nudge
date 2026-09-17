using Nudge.Shared.Core.Localization;

namespace Nudge.Bot.Localization;

public interface IBotMessageResolver
{
    Task<string> GetWelcomeMessageAsync(string firstName, bool isReturningUser, Language language, CancellationToken cancellationToken);

    Task<string> GetUnknownCommandMessageAsync(Language language, CancellationToken cancellationToken);

    Task<string> GetInternalErrorMessageAsync(Language language, CancellationToken cancellationToken);
}
