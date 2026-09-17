using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Nudge.Identity.Users.ValueObjects;
using Nudge.Learning.Data;
using Nudge.Learning.Decks.Models;
using Nudge.Learning.Decks.ValueObjects;
using Nudge.Learning.Users;
using Nudge.Learning.Users.Features.CreateOrActivateDefaultDeck.V1;
using Nudge.Shared.Core.Utils;
using Xunit;

namespace Nudge.UnitTests.Learning.Users.Features.CreateOrActivateDefaultDeck.V1;

public class CreateOrActivateDefaultDeckHandlerTests
{
    private class SequentialIdGenerator : IIdGenerator<long>
    {
        private long _next = 1;

        public long CreateId() => _next++;
    }

    private static LearningDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<LearningDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new LearningDbContext(options);
    }

    private static CreateOrActivateDefaultDeckHandler CreateHandler(LearningDbContext dbContext, string deckName = "Inbox")
    {
        return new CreateOrActivateDefaultDeckHandler(
            NullLogger<CreateOrActivateDefaultDeckHandler>.Instance,
            dbContext,
            new SequentialIdGenerator(),
            Options.Create(new DefaultDeckOptions { Name = deckName }));
    }

    [Fact]
    public async Task Handle_When_NoDeckExistsForUser_Then_CreatesDefaultDeck()
    {
        await using var dbContext = CreateDbContext();
        var handler = CreateHandler(dbContext);

        await handler.Handle(new CreateOrActivateDefaultDeckCommand(UserId: 1L), CancellationToken.None);
        await dbContext.SaveChangesAsync();

        var deck = Assert.Single(dbContext.Decks);
        Assert.Equal(UserId.Of(1L), deck.UserId);
        Assert.Equal("Inbox", deck.Title);
        Assert.False(deck.IsArchived);
        Assert.False(deck.IsDeleted);
    }

    [Fact]
    public async Task Handle_When_DeckAlreadyArchived_Then_ReactivatesInsteadOfCreatingDuplicate()
    {
        await using var dbContext = CreateDbContext();

        var existingDeck = Deck.Create(DeckId.Of(1L), UserId.Of(1L), DeckTitle.Of("Inbox"), DeckDescription.Of(null), isArchived: true);
        await dbContext.AddAsync(existingDeck);
        await dbContext.SaveChangesAsync();

        var handler = CreateHandler(dbContext);
        await handler.Handle(new CreateOrActivateDefaultDeckCommand(UserId: 1L), CancellationToken.None);
        await dbContext.SaveChangesAsync();

        var deck = Assert.Single(dbContext.Decks.IgnoreQueryFilters());
        Assert.False(deck.IsArchived);
        Assert.False(deck.IsDeleted);
    }

    [Fact]
    public async Task Handle_When_ActiveDeckAlreadyExists_Then_DoesNotCreateDuplicate()
    {
        await using var dbContext = CreateDbContext();

        var existingDeck = Deck.Create(DeckId.Of(1L), UserId.Of(1L), DeckTitle.Of("Inbox"), DeckDescription.Of(null), isArchived: false);
        await dbContext.AddAsync(existingDeck);
        await dbContext.SaveChangesAsync();

        var handler = CreateHandler(dbContext);
        await handler.Handle(new CreateOrActivateDefaultDeckCommand(UserId: 1L), CancellationToken.None);
        await dbContext.SaveChangesAsync();

        Assert.Single(dbContext.Decks.IgnoreQueryFilters());
    }

    [Fact]
    public async Task Handle_When_CustomDeckNameConfigured_Then_UsesConfiguredName()
    {
        await using var dbContext = CreateDbContext();
        var handler = CreateHandler(dbContext, deckName: "My Deck");

        await handler.Handle(new CreateOrActivateDefaultDeckCommand(UserId: 1L), CancellationToken.None);
        await dbContext.SaveChangesAsync();

        var deck = Assert.Single(dbContext.Decks);
        Assert.Equal("My Deck", deck.Title);
    }
}
