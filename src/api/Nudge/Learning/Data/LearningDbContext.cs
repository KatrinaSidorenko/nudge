using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Nudge.Learning;
using Nudge.Learning.Decks.Models;
using Nudge.Shared.EFCore;

namespace Nudge.Learning.Data;

public class LearningDbContext : AppDbContextBase
{
    private const string _schema = "learning";

    public LearningDbContext(DbContextOptions options, ILogger<LearningDbContext>? logger = null)
        : base(options, logger)
    {
    }

    public DbSet<Deck> Decks => Set<Deck>();

    // public DbSet<Card> Decks => Set<Deck>();
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.HasDefaultSchema(_schema);
        builder.ApplyConfigurationsFromNamespaceOf<LearningRoot>();
        base.OnModelCreating(builder);
        builder.FilterSoftDeletedProperties(); // can be ignored in query building by adding .IgnoreQueryFilters()
        builder.ToSnakeCaseTables();
    }
}
