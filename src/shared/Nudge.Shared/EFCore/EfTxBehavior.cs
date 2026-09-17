using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Nudge.Shared.Core.Model;

namespace Nudge.Shared.EFCore;

// ref: https://github.com/meysamhadeli/booking-microservices/blob/main/src/BuildingBlocks/EFCore/EfTxBehavior.cs
//
// Saves whatever the handler changed on every registered DbContext, then logs the domain events
// that were raised on tracked aggregates during the request. There's no dispatcher yet (see the
// "Further" section of the user-upsert task) — events are only logged for now, not published.
public class EfTxBehavior<TRequest, TResponse>(IEnumerable<DbContext> dbContexts, ILogger<EfTxBehavior<TRequest, TResponse>> logger) : IPipelineBehavior<TRequest, TResponse>
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
                foreach (var domainEvent in aggregate.ClearDomainEvents())
                {
                    logger.LogInformation(
                        "Domain event {EventType} ({EventId}) occurred on {OccurredOn}",
                        domainEvent.EventType,
                        domainEvent.EventId,
                        domainEvent.OccurredOn);
                }
            }
        }

        return response;
    }
}
