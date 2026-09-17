namespace Nudge.Shared.EFCore;

public interface IUnitOfWork
{
    // Saves every registered DbContext's pending changes, then publishes the domain events
    // raised on their tracked aggregates. Keeps draining until nothing is left pending, so
    // events raised by a handler reacting to an earlier event in the same batch aren't dropped.
    Task SaveChangesAndPublishEventsAsync(CancellationToken cancellationToken = default);
}
