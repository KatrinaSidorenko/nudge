using Asp.Versioning;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;

namespace Nudge.Shared.OpenApi;

public static class OpenApiExtensions
{
    public static IServiceCollection AddOpenApi(this IServiceCollection services, string[] versions)
    {
        services.AddSwaggerGen();
        services.ConfigureOptions<ConfigureSwaggerOptions>();

        services.AddApiVersioning(o =>
        {
            o.DefaultApiVersion = new ApiVersion(1.0);
            o.AssumeDefaultVersionWhenUnspecified = true;
            o.ReportApiVersions = true;
            o.ApiVersionReader = ApiVersionReader.Combine(
                   new UrlSegmentApiVersionReader(),
                   new QueryStringApiVersionReader("api-version"));
        }).AddApiExplorer(s =>
        {
            s.GroupNameFormat = "'v'VVV";
            s.SubstituteApiVersionInUrl = true;
        });

        return services;
    }

    public static IApplicationBuilder UseOpenApi(this WebApplication app)
    {
        app.UseSwaggerUI(
               options =>
               {
                   var descriptions = app.DescribeApiVersions();

                   // build a swagger endpoint for each discovered API version
                   foreach (var description in descriptions)
                   {
                       var openApiUrl = $"/openapi/{description.GroupName}.json";
                       var name = description.GroupName.ToUpperInvariant();
                       options.SwaggerEndpoint(openApiUrl, name);
                   }
               });
        return app;
    }
}
