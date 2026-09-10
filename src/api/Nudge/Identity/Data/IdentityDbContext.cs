using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Nudge.Identity.Users.Models;
using Nudge.Shared.EFCore;

namespace Nudge.Identity.Data;

public class IdentityDbContext : AppDbContextBase
{
    private const string _schema = "identity";

    public IdentityDbContext(DbContextOptions options, ILogger<IdentityDbContext>? logger = null)
        : base(options, logger)
    {
    }

    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.HasDefaultSchema(_schema);

        // Nudge/Learning and Nudge/Identity share one assembly — scope by namespace so this
        // context doesn't pick up the other module's IEntityTypeConfiguration<T> and pull its
        // tables into the identity schema (breaks schema-per-module isolation otherwise).
        builder.ApplyConfigurationsFromAssembly(
            Assembly.GetExecutingAssembly(),
            t => t.Namespace?.StartsWith("Nudge.Identity.", StringComparison.Ordinal) == true);
        base.OnModelCreating(builder);
        builder.FilterSoftDeletedProperties(); // can be ignored in query building by adding .IgnoreQueryFilters()
        builder.ToSnakeCaseTables();
    }
}
