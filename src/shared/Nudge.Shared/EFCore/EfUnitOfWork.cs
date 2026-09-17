using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Nudge.Shared.Core.Event;
using Nudge.Shared.Core.Model;

namespace Nudge.Shared.EFCore;

public class EfUnitOfWork(IEnumerable<DbContext> dbContexts, IEventBus eventBus, ILogger<EfUnitOfWork> logger) : IUnitOfWork
{
    public async Task SaveChangesAndPublishEventsAsync(CancellationToken cancellationToken = default)
    {
        foreach (var dbContext in dbContexts)
        {
            await SaveAndDrainEventsAsync(dbContext, cancellationToken);
        }
    }

    // Loops save-then-publish until a pass leaves nothing pending — a handler reacting to one
    // event can raise another (or track a new aggregate) on the same DbContext, and a single
    // pass would silently drop it.
    private async Task SaveAndDrainEventsAsync(DbContext dbContext, CancellationToken cancellationToken)
    {
        while (true)
        {
            if (dbContext.ChangeTracker.HasChanges())
            {
                await dbContext.SaveChangesAsync(cancellationToken);
            }

            var aggregatesWithEvents = dbContext.ChangeTracker.Entries<IAggregate>()
                .Select(entry => entry.Entity)
                .Where(aggregate => aggregate.DomainEvents.Count > 0)
                .ToList();

            if (aggregatesWithEvents.Count == 0)
            {
                return;
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
    }
}
