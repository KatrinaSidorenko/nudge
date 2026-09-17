using System.Reflection;
using Asp.Versioning;
using Asp.Versioning.Builder;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Nudge.Shared.Core.Utils;
using Scrutor;

namespace Nudge.Shared.Core.Api;

public static class MinimalApiExtensions
{

    public static IServiceCollection AddMinimalEndpoints(
        this WebApplicationBuilder applicationBuilder,
        ServiceLifetime lifetime = ServiceLifetime.Scoped,
        params Assembly[] assemblies)
    {

        var scanAssemblies = assemblies.Any()
            ? assemblies
            : TypeProvider.GetReferencedAssemblies(Assembly.GetCallingAssembly())
                .Concat(TypeProvider.GetApplicationPartAssemblies(Assembly.GetCallingAssembly()))
                .Distinct()
                .ToArray();

        applicationBuilder.Services.Scan(scan => scan
            .FromAssemblies(scanAssemblies)
            .AddClasses(classes => classes.AssignableTo(typeof(IMinimalEndpoint)))
            .UsingRegistrationStrategy(RegistrationStrategy.Append)
            .As<IMinimalEndpoint>()
            .WithLifetime(lifetime));

        return applicationBuilder.Services;
    }

    /// <summary>
    /// Map Minimal Endpoints
    /// </summary>
    /// <name>builder.</name>
    /// <returns>IEndpointRouteBuilder.</returns>
    public static IEndpointRouteBuilder MapMinimalEndpoints(this IEndpointRouteBuilder builder)
    {
        var scope = builder.ServiceProvider.CreateScope();

        var endpoints = scope.ServiceProvider.GetServices<IMinimalEndpoint>();

        // Every IMinimalEndpoint calls .HasApiVersion(...) on the builder it's given, which
        // requires the endpoint to belong to an ApiVersionSet — build one shared set here so
        // individual endpoint classes don't each need to wire their own.
        var versionSet = builder.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1.0))
            .ReportApiVersions()
            .Build();

        var versionedBuilder = builder.MapGroup(string.Empty).WithApiVersionSet(versionSet);

        foreach (var endpoint in endpoints)
        {
            endpoint.MapEndpoint(versionedBuilder);
        }

        return builder;
    }
}
