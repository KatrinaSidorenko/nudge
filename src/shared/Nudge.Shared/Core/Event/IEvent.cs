// using MediatR;

namespace Nudge.Shared.Core.Event;

public interface IEvent /*: INotification*/
{
    // Guid EventId => NewId.NextGuid(); // mass transit stuff?? why masstransit generation?
    Guid EventId => Guid.NewGuid();

    public DateTime OccurredOn => DateTime.Now;

    public string EventType => GetType().AssemblyQualifiedName;
}
