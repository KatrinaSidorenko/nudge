using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Nudge.Identity.Data;
using Nudge.Identity.Users.Models;
using Nudge.Identity.Users.ValueObjects;
using Nudge.Shared.Core.Event;
using Nudge.Shared.EFCore;
using Xunit;

namespace Nudge.UnitTests.Shared.EFCore;

public class EfUnitOfWorkTests
{
    private class FakeEventBus(Func<IEnumerable<IEvent>, CancellationToken, Task> onPublish) : IEventBus
    {
        public Task PublishAsync(IEnumerable<IEvent> events, CancellationToken cancellationToken = default) =>
            onPublish(events, cancellationToken);
    }

    private static IdentityDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<IdentityDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new IdentityDbContext(options);
    }

    [Fact]
    public async Task SaveChangesAndPublishEventsAsync_When_HandlingAnEventRaisesAnotherOnTheSameAggregate_Then_DrainsAndPublishesBoth()
    {
        await using var dbContext = CreateDbContext();
        var user = User.Create(UserId.Of(1L), TelegramUserId.Of(1L), "Kate", null, null, null);
        await dbContext.AddAsync(user);

        var publishedEvents = new List<IEvent>();
        var bus = new FakeEventBus((events, _) =>
        {
            foreach (var @event in events)
            {
                publishedEvents.Add(@event);

                // Simulate a handler reacting to the first event by causing the same still-tracked
                // aggregate to raise a second one — this is exactly what a single, non-draining pass
                // would silently drop.
                if (@event is UserUpsertedEvent { IsNewUser: true })
                {
                    user.UpdateProfile("Kate", null, null, null);
                }
            }

            return Task.CompletedTask;
        });

        var unitOfWork = new EfUnitOfWork([dbContext], bus, NullLogger<EfUnitOfWork>.Instance);

        await unitOfWork.SaveChangesAndPublishEventsAsync();

        Assert.Equal(2, publishedEvents.Count);
        Assert.True(((UserUpsertedEvent)publishedEvents[0]).IsNewUser);
        Assert.False(((UserUpsertedEvent)publishedEvents[1]).IsNewUser);
    }

    [Fact]
    public async Task SaveChangesAndPublishEventsAsync_When_NoChangesOrEventsPending_Then_DoesNotPublish()
    {
        await using var dbContext = CreateDbContext();

        var publishedEvents = new List<IEvent>();
        var bus = new FakeEventBus((events, _) =>
        {
            publishedEvents.AddRange(events);
            return Task.CompletedTask;
        });

        var unitOfWork = new EfUnitOfWork([dbContext], bus, NullLogger<EfUnitOfWork>.Instance);

        await unitOfWork.SaveChangesAndPublishEventsAsync();

        Assert.Empty(publishedEvents);
    }
}
