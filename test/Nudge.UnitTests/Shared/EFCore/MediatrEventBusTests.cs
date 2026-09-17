using MediatR;
using Nudge.Shared.Core.Event;
using Nudge.Shared.EFCore;
using Xunit;

namespace Nudge.UnitTests.Shared.EFCore;

public class MediatrEventBusTests
{
    private record TestEvent(int Order) : IEvent;

    private class RecordingPublisher : IPublisher
    {
        public List<int> PublishedOrder { get; } = [];

        public Task Publish(object notification, CancellationToken cancellationToken = default)
        {
            PublishedOrder.Add(((TestEvent)notification).Order);
            return Task.CompletedTask;
        }

        public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
            where TNotification : INotification
        {
            PublishedOrder.Add(((TestEvent)(object)notification!).Order);
            return Task.CompletedTask;
        }
    }

    [Fact]
    public async Task PublishAsync_When_MultipleEventsGiven_Then_PublishesAllOfThemInOrder()
    {
        var publisher = new RecordingPublisher();
        var bus = new MediatrEventBus(publisher);
        var events = new IEvent[] { new TestEvent(1), new TestEvent(2), new TestEvent(3) };

        await bus.PublishAsync(events);

        Assert.Equal([1, 2, 3], publisher.PublishedOrder);
    }
}
