namespace Nudge.Shared.Core.Event;

public interface IEventBus
{
    Task PublishAsync(IEnumerable<IEvent> events, CancellationToken cancellationToken = default);
}
