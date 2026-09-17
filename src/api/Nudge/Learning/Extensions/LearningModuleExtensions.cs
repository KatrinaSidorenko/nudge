using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Nudge.Learning.Data;
using Nudge.Learning.Decks;

namespace Nudge.Learning.Extensions;

public static class LearningModuleExtensions
{
    public static WebApplicationBuilder AddLearningModule(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<LearningDbOptions>(builder.Configuration.GetSection(LearningDbOptions.SectionName));
        builder.Services.Configure<DefaultDeckOptions>(builder.Configuration.GetSection(DefaultDeckOptions.SectionName));

        builder.Services.AddDbContext<LearningDbContext>((sp, options) =>
            options.UseNpgsql(sp.GetRequiredService<IOptions<LearningDbOptions>>().Value.ConnectionString));

        // EfUnitOfWork resolves IEnumerable<DbContext> to save/publish every module's changes;
        // AddDbContext<T> only registers T itself, so it's also exposed as the base type here.
        builder.Services.AddScoped<DbContext>(sp => sp.GetRequiredService<LearningDbContext>());

        return builder;
    }
}
