using Microsoft.EntityFrameworkCore;
using Nudge.Shared.EFCore;

namespace Nudge.Learning.Data;

public class LearningDbContextFactory : DesignTimeDbContextFactoryBase<LearningDbContext, LearningDbOptions>
{
    protected override string SectionName => LearningDbOptions.SectionName;

    protected override LearningDbContext CreateDbContext(DbContextOptions<LearningDbContext> options) => new(options);
}
