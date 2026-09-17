using System.Reflection;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Nudge.Learning;
using Nudge.Shared.Core.CQRS;
using Nudge.Shared.EFCore;

namespace Nudge.Api.Extensions;

public static class MediatRExtensions
{
    public static IServiceCollection AddCustomMediatR(this IServiceCollection services)
    {
        // Handlers live in Nudge (LearningRoot's assembly); this assembly (Nudge.Api) is scanned
        // too in case an endpoint-side pipeline behavior is added later.
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(LearningRoot).Assembly, Assembly.GetExecutingAssembly()));

        // Registration order = outer-to-inner pipeline: every request is logged, then validated,
        // then handled and its changes/domain events are saved/logged by EfTxBehavior.
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(EfTxBehavior<,>));
        //services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CachingBehavior<,>));
        //services.AddTransient(typeof(IPipelineBehavior<,>), typeof(InvalidateCachingBehavior<,>));

        return services;
    }
}
