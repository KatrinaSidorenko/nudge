using FluentValidation;
using Mapster;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;
using Nudge.Shared.Core.Api;

namespace Nudge.Identity.Users.Features.UpsertUser.V1;

public class UpsertUserRequestDto
{
    [JsonProperty("telegramId")]
    public long? TelegramId { get; set; }

    [JsonProperty("firstName")]
    public string FirstName { get; set; }

    [JsonProperty("lastName")]
    public string? LastName { get; set; }

    [JsonProperty("username")]
    public string? Username { get; set; }

    [JsonProperty("languageCode")]
    public string? LanguageCode { get; set; }
}

public class UpsertUserResponseDto
{
    [JsonProperty("id")]
    public long Id { get; set; }

    [JsonProperty("isNewUser")]
    public bool IsNewUser { get; set; }
}

public class UpsertUserRequestDtoValidator : AbstractValidator<UpsertUserRequestDto>
{
    public UpsertUserRequestDtoValidator()
    {
        // Telegram is the only supported identity today; email/password is future work (see the
        // user-upsert task's "Further" section), which is why the field itself is nullable.
        RuleFor(x => x.TelegramId).NotNull();
        RuleFor(x => x.FirstName).NotNull().NotEmpty();
    }
}

public class UpsertUserEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost(UsersApiPaths.UpsertUser, async (UpsertUserRequestDto request, IMapper mapper, IMediator mediator, CancellationToken ct) =>
        {
            var command = mapper.Map<UpsertUserCommand>(request);
            var result = await mediator.Send(command, ct);
            var response = result.Adapt<UpsertUserResponseDto>();
            return Results.Ok(response);
        })
            // todo: add authorization once a Telegram WebApp/JWT auth scheme lands
            .WithValidation<UpsertUserRequestDto>()
            .HasApiVersion(1.0);

        return builder;
    }
}
