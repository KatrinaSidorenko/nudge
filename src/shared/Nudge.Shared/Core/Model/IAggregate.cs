using Nudge.Shared.Core.Event;

namespace Nudge.Shared.Core.Model;

public interface IAggregate<T> : IAggregate, IEntity<T>
{
}

public interface IAggregate : IEntity
{
    IReadOnlyList<IDomainEvent> DomainEvents { get; }

    IEvent[] ClearDomainEvents();
}
