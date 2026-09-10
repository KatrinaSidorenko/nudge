using Microsoft.EntityFrameworkCore;
using Nudge.Shared.EFCore;

namespace Nudge.Identity.Data;

public class IdentityDbContextFactory : DesignTimeDbContextFactoryBase<IdentityDbContext, IdentityDbOptions>
{
    protected override string SectionName => IdentityDbOptions.SectionName;

    protected override IdentityDbContext CreateDbContext(DbContextOptions<IdentityDbContext> options) => new(options);
}
