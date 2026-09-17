using MediatR;
using Nudge.Shared.Core.Event;

namespace Nudge.Shared.EFCore;

// In-memory IEventBus, swappable later (e.g. for a RabbitMqEventBus) without touching callers.
public class MediatrEventBus(IPublisher publisher) : IEventBus
{
    public async Task PublishAsync(IEnumerable<IEvent> events, CancellationToken cancellationToken = default)
    {
        foreach (var @event in events)
        {
            await publisher.Publish(@event, cancellationToken);
        }
    }
}
