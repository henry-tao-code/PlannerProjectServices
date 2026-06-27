using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace ProjectPlanner.API.Filters;

public class ValidationFilter<T>(IValidator<T>? validator = null) : IEndpointFilter where T : class
{
    private readonly IValidator<T>? _validator = validator;

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        // If no validator is registered for this specific DTO type, just pass through
        if (_validator == null) return await next(context);

        // Find the argument matching the DTO type in the endpoint parameters
        var argToValidate = context.Arguments.FirstOrDefault(x => x is T) as T;

        if (argToValidate != null)
        {
            var validationResult = await _validator.ValidateAsync(argToValidate);

            if (!validationResult.IsValid)
            {
                // Instantly short-circuits and returns a clean HTTP 400 Validation Problem
                return TypedResults.ValidationProblem(validationResult.ToDictionary());
            }
        }

        return await next(context);
    }
}