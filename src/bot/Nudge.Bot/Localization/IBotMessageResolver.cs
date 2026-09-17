namespace Nudge.Bot.Localization;

public interface IBotMessageResolver
{
    Task<string> GetWelcomeMessageAsync(string firstName, string? languageCode, CancellationToken cancellationToken);

    Task<string> GetUnknownCommandMessageAsync(string? languageCode, CancellationToken cancellationToken);

    Task<string> GetInternalErrorMessageAsync(string? languageCode, CancellationToken cancellationToken);
}
