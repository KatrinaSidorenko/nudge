using Microsoft.Extensions.Logging;
using Nudge.Learning.Data;
using Nudge.Learning.Decks.Models;
using Nudge.Learning.Decks.ValueObjects;
using Nudge.Shared.Core.CQRS;
using Nudge.Shared.Core.Utils;

namespace Nudge.Learning.Decks.Features.CreateDeck.V1;

public record CreateDeckCommand(string Title, string? Description) : ICommand<CreateDeckResult>;
public record CreateDeckResult(long Id);

public class CreateDeckHandler(ILogger logger, LearningDbContext dbContext, IIdGenerator<long> idGenerator) : ICommandHandler<CreateDeckCommand, CreateDeckResult>
{
    public async Task<CreateDeckResult> Handle(CreateDeckCommand request, CancellationToken cancellationToken)
    {
        var deck = Deck.Create(DeckId.Of(idGenerator.CreateId()), DeckTitle.Of(request.Title), DeckDescription.Of(request.Description), isArchived: false);
        await dbContext.AddAsync(deck, cancellationToken);

        return new CreateDeckResult(deck.Id);
    }
}
