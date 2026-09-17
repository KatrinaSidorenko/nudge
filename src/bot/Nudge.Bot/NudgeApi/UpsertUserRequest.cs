using System.Text.Json.Serialization;

namespace Nudge.Bot.NudgeApi;

// Wire-compatible with Nudge.Identity.Users.Features.UpsertUser.V1.UpsertUserRequestDto /
// UpsertUserResponseDto on the API side. Kept as plain DTOs here since the bot doesn't reference
// the API project.
public record UpsertUserRequest(
    [property: JsonPropertyName("telegramId")] long TelegramId,
    [property: JsonPropertyName("firstName")] string FirstName,
    [property: JsonPropertyName("lastName")] string? LastName,
    [property: JsonPropertyName("username")] string? Username,
    [property: JsonPropertyName("languageCode")] string? LanguageCode);

public record UpsertUserResponse(
    [property: JsonPropertyName("id")] long Id,
    [property: JsonPropertyName("isNewUser")] bool IsNewUser);
