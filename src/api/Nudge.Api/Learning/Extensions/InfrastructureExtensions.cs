using System.Reflection;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nudge.Shared.Mapster;
using Nudge.Shared.OpenApi;

namespace Nudge.Learning.Extensions;

public static class InfrastructureExtensions
{
    private static string[] _apiVersions = new[] { "v1" };

    public static WebApplicationBuilder AddInfrastructure(this WebApplicationBuilder builder)
    {
        builder.Services.AddOpenApi(_apiVersions);
        builder.Services.AddProblemDetails();

        // Endpoints, DTOs and validators live in Nudge (LearningRoot's assembly) now; this
        // assembly (Nudge.Api) is scanned too in case a host-side validator is added later.
        builder.Services.AddValidatorsFromAssembly(typeof(LearningRoot).Assembly);
        builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        builder.Services.AddCustomMapster(typeof(LearningRoot).Assembly, Assembly.GetExecutingAssembly());
        builder.Services.AddCustomMediatR();

        return builder;
    }

    public static WebApplication UseInfrastructure(this WebApplication builder)
    {
        if (builder.Environment.IsDevelopment())
        {
            builder.UseOpenApi();
        }

        return builder;
    }
}
