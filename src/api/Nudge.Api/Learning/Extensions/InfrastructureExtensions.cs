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

        // Endpoints, DTOs and validators live in this assembly (Nudge.Api); LearningRoot's
        // assembly (Nudge) holds only domain/command types now, so both are scanned.
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
