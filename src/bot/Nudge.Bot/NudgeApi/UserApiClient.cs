using System.Net.Http.Json;

namespace Nudge.Bot.NudgeApi;

public class UserApiClient(HttpClient httpClient) : IUserApiClient
{
    public async Task<UpsertUserResponse> UpsertUserAsync(UpsertUserRequest request, CancellationToken cancellationToken)
    {
        var response = await httpClient.PostAsJsonAsync("api/v1/users", request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<UpsertUserResponse>(cancellationToken);
        return result ?? throw new InvalidOperationException("Nudge.Api returned an empty upsert-user response.");
    }
}
