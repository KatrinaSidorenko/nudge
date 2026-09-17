using FluentValidation;
using Mapster;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;
using Nudge.Learning.Decks.BussinessRules;
using Nudge.Shared.Core.Api;

namespace Nudge.Learning.Decks.Features.CreateDeck.V1;

public class CreateDeckRequestDto
{
    // Client-supplied for now — there's no authenticated-user context yet (see the todo below).
    [JsonProperty("userId")]
    public long UserId { get; set; }

    [JsonProperty("title")]
    public string Title { get; set; }

    [JsonProperty("description")]
    public string? Description { get; set; }
}

public class CreateDeckResponseDto
{
    [JsonProperty("id")]
    public long Id { get; set; }
}

public class CreateDeckRequestDtoValidator : AbstractValidator<CreateDeckRequestDto>
{
    public CreateDeckRequestDtoValidator()
    {
        RuleFor(x => x.UserId).GreaterThan(0);
        RuleFor(x => x.Title).NotNull().NotEmpty().MaximumLength(DeckTitleShouldBeLessThanNCharacters.Length);
        RuleFor(x => x.Description).MaximumLength(DeckDescriptionShouldBeLessThanNCharacters.Length);
    }
}

public class CreateDeckEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost(DecksApiPaths.CreateNewDeck, async (CreateDeckRequestDto request, IMapper mapper, IMediator mediator, CancellationToken ct) =>
        {
            var command = mapper.Map<CreateDeckCommand>(request);
            var result = await mediator.Send(command, ct);
            var response = result.Adapt<CreateDeckResponseDto>();
            return Results.CreatedAtRoute("GetDeckById", new { id = result.Id }, response); // not sure about magic string
        })
            // todo: add authorization — once wired up, resolve UserId from the principal instead of the request body
            .WithValidation<CreateDeckRequestDto>()
            .HasApiVersion(1.0);

        return builder;
    }
}
