using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Nudge.Identity;
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
        builder.ApplyConfigurationsFromNamespaceOf<IdentityRoot>();
        base.OnModelCreating(builder);
        builder.FilterSoftDeletedProperties(); // can be ignored in query building by adding .IgnoreQueryFilters()
        builder.ToSnakeCaseTables();
    }
}
