using System.Reflection;
using FluentValidation;
using IdGen.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nudge.Identity.Extensions;
using Nudge.Learning;
using Nudge.Learning.Extensions;
using Nudge.Shared.Core.Api;
using Nudge.Shared.Core.Utils;
using Nudge.Shared.Mapster;
using Nudge.Shared.OpenApi;

namespace Nudge.Api.Extensions;

public static class InfrastructureExtensions
{
    private static readonly string[] _apiVersions = new[] { "v1" };

    public static WebApplicationBuilder AddInfrastructure(this WebApplicationBuilder builder)
    {
        builder.Services.AddOpenApi(_apiVersions);
        builder.Services.AddProblemDetails();

        // Each module owns its own DbContext wiring; the host just adds them together.
        builder.AddLearningModule();
        builder.AddIdentityModule();

        // Snowflake IDs for every aggregate's client-generated Id (see CreateDeckHandler,
        // UpsertUserHandler). The generator id just needs to be stable per running instance;
        // revisit once this runs as more than a single instance.
        builder.Services.AddIdGen(generatorId: 0);
        builder.Services.AddSingleton<IIdGenerator<long>, SnowflakeIdGeneratorAdapter>();

        // Endpoints, DTOs and validators for every module live in Nudge (LearningRoot's
        // assembly) now; this assembly (Nudge.Api) is scanned too in case a host-side validator
        // is added later.
        builder.Services.AddValidatorsFromAssembly(typeof(LearningRoot).Assembly);
        builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        builder.Services.AddCustomMapster(typeof(LearningRoot).Assembly, Assembly.GetExecutingAssembly());
        builder.Services.AddCustomMediatR();

        builder.AddMinimalEndpoints();

        return builder;
    }

    public static WebApplication UseInfrastructure(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseOpenApi();
        }

        app.MapMinimalEndpoints();

        return app;
    }
}
