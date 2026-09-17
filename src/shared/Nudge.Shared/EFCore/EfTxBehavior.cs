using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Nudge.Shared.Core.Event;
using Nudge.Shared.Core.Model;

namespace Nudge.Shared.EFCore;

// ref: https://github.com/meysamhadeli/booking-microservices/blob/main/src/BuildingBlocks/EFCore/EfTxBehavior.cs
//
// Saves whatever the handler changed on every registered DbContext, then logs and publishes the
// domain events that were raised on tracked aggregates during the request.
public class EfTxBehavior<TRequest, TResponse>(IEnumerable<DbContext> dbContexts, IEventBus eventBus, ILogger<EfTxBehavior<TRequest, TResponse>> logger) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var response = await next(cancellationToken);

        foreach (var dbContext in dbContexts)
        {
            var aggregatesWithEvents = dbContext.ChangeTracker.Entries<IAggregate>()
                .Select(entry => entry.Entity)
                .Where(aggregate => aggregate.DomainEvents.Count > 0)
                .ToList();

            if (dbContext.ChangeTracker.HasChanges())
            {
                await dbContext.SaveChangesAsync(cancellationToken);
            }

            foreach (var aggregate in aggregatesWithEvents)
            {
                var domainEvents = aggregate.ClearDomainEvents();

                foreach (var domainEvent in domainEvents)
                {
                    logger.LogInformation(
                        "Domain event {EventType} ({EventId}) occurred on {OccurredOn}",
                        domainEvent.EventType,
                        domainEvent.EventId,
                        domainEvent.OccurredOn);
                }

                // Published sequentially, in raised order, one event at a time (see MediatrEventBus) —
                // every event must run, not just the first, or the pub/sub model is pointless.
                await eventBus.PublishAsync(domainEvents, cancellationToken);
            }
        }

        return response;
    }
}
