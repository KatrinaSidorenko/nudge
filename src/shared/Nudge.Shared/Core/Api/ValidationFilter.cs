using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Nudge.Shared.Core.Api;

public class ValidationFilter<T> : IEndpointFilter
    where T : class
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var validator = context.HttpContext.RequestServices.GetService<IValidator<T>>();
        if (validator is null)
        {
            return await next(context);
        }

        var argument = context.Arguments.OfType<T>().FirstOrDefault();
        if (argument is null)
        {
            return TypedResults.Problem(
                detail: $"Request body missing or invalid for parameter type '{typeof(T).Name}'.",
                statusCode: StatusCodes.Status400BadRequest
            );
        }

        var validationResult = await validator.ValidateAsync(argument, context.HttpContext.RequestAborted);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.ToDictionary();
            return TypedResults.ValidationProblem(errors);
        }

        return await next(context);
    }
}

public static class RouteGroupBuilderExtensions
{
    public static RouteHandlerBuilder WithValidation<T>(this RouteHandlerBuilder builder)
        where T : class
        => builder.AddEndpointFilter<ValidationFilter<T>>();
}
