using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nudge.Identity.Users.ValueObjects;
using Nudge.Learning.Data;
using Nudge.Learning.Decks.Models;
using Nudge.Learning.Decks.ValueObjects;
using Nudge.Shared.Core.CQRS;
using Nudge.Shared.Core.Utils;

namespace Nudge.Learning.Users.Features.CreateOrActivateDefaultDeck.V1;

public record CreateOrActivateDefaultDeckCommand(long UserId) : ICommand;

public class CreateOrActivateDefaultDeckHandler(
    ILogger<CreateOrActivateDefaultDeckHandler> logger,
    LearningDbContext dbContext,
    IIdGenerator<long> idGenerator,
    IOptions<DefaultDeckOptions> options) : ICommandHandler<CreateOrActivateDefaultDeckCommand>
{
    public async Task<Unit> Handle(CreateOrActivateDefaultDeckCommand request, CancellationToken cancellationToken)
    {
        var userId = UserId.Of(request.UserId);
        var deckName = options.Value.Name;

        // IgnoreQueryFilters: the global soft-delete filter would otherwise hide an
        // archived/deleted default deck, and we need to find it in order to reactivate it.
        var existingDeck = await dbContext.Decks.IgnoreQueryFilters()
            .SingleOrDefaultAsync(d => d.UserId.Value == userId.Value && d.Title.Value == deckName, cancellationToken);

        if (existingDeck is null)
        {
            logger.LogInformation("Creating default {DeckName} deck for user {UserId}", deckName, request.UserId);

            var deck = Deck.Create(DeckId.Of(idGenerator.CreateId()), userId, DeckTitle.Of(deckName), DeckDescription.Of(null), isArchived: false);
            await dbContext.AddAsync(deck, cancellationToken);
        }
        else if (existingDeck.IsArchived || existingDeck.IsDeleted)
        {
            logger.LogInformation("Reactivating default {DeckName} deck {DeckId} for user {UserId}", deckName, existingDeck.Id, request.UserId);

            existingDeck.Activate();
        }

        return Unit.Value;
    }
}
