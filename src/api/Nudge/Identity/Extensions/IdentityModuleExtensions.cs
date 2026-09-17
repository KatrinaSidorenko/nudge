using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Nudge.Identity.Data;

namespace Nudge.Identity.Extensions;

public static class IdentityModuleExtensions
{
    public static WebApplicationBuilder AddIdentityModule(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<IdentityDbOptions>(builder.Configuration.GetSection(IdentityDbOptions.SectionName));

        builder.Services.AddDbContext<IdentityDbContext>((sp, options) =>
            options.UseNpgsql(sp.GetRequiredService<IOptions<IdentityDbOptions>>().Value.ConnectionString));

        // EfTxBehavior resolves IEnumerable<DbContext> to save/log every module's changes;
        // AddDbContext<T> only registers T itself, so it's also exposed as the base type here.
        builder.Services.AddScoped<DbContext>(sp => sp.GetRequiredService<IdentityDbContext>());

        return builder;
    }
}
