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

        builder.Services.AddValidatorsFromAssemblyContaining<LearningRoot>();

        builder.Services.AddCustomMapster(typeof(LearningRoot).Assembly);
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
