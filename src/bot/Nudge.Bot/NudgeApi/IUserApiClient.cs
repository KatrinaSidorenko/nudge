namespace Nudge.Bot.NudgeApi;

public interface IUserApiClient
{
    Task<UpsertUserResponse> UpsertUserAsync(UpsertUserRequest request, CancellationToken cancellationToken);
}
