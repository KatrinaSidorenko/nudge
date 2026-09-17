using Nudge.Bot.NudgeApi;
using Nudge.Shared.Core.Localization;

namespace Nudge.Bot.Users;

// Thin wrapper around IUserApiClient so callers (command handlers) depend on this instead of the
// HTTP transport directly. If Nudge.Api ever exposes users over gRPC instead, only this
// implementation (and the DI registration in Program.cs) needs to change.
public class UserService(IUserApiClient userApiClient) : IUserService
{
    public async Task<UpsertUserResult> UpsertUserAsync(long telegramId, string firstName, string? lastName, string? username, Language languageCode, CancellationToken cancellationToken)
    {
        var request = new UpsertUserRequest(telegramId, firstName, lastName, username, languageCode);

        // TODO: add a retry policy (e.g. Polly) once Nudge.Api is deployed somewhere with
        // predictable reachability/latency; a single transient network blip currently fails the
        // whole /start call.
        var response = await userApiClient.UpsertUserAsync(request, cancellationToken);

        return new UpsertUserResult(response.Id, response.IsNewUser);
    }
}
