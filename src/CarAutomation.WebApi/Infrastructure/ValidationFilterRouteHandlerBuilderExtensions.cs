using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO.Pipelines;
using System.Reflection;
using System.Security.Claims;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.AspNetCore.Http;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Validation extension methods for <see cref="RouteHandlerBuilder"/>.
/// </summary>
/// <remarks>
/// This is adapted from https://github.com/DamianEdwards/MinimalApis.Extensions/blob/main/src/MinimalApis.Extensions/Filters/ValidationFilterRouteHandlerBuilderExtensions.cs
/// as suggested from https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/min-api-filters?view=aspnetcore-9.0.
/// I felt like I didn't want to pull down the whole MiniValidator library as I only needed the filter and .NET 10 will have built-in validation later this year.
/// </remarks>
public static class ValidationFilterRouteHandlerBuilderExtensions
{
    public static TBuilder WithValidation<TBuilder>(this TBuilder endpoint)
        where TBuilder : IEndpointConventionBuilder
    {
        endpoint.Add(builder =>
        {
            var loggerFactory = builder.ApplicationServices.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger("MinimalApis.Extensions.Filters.ValidationRouteHandlerFilter");

            var methodInfo = builder.Metadata.OfType<MethodInfo>().FirstOrDefault();

            if (methodInfo is null)
            {
                return;
            }

            var isService = builder.ApplicationServices.GetService<IServiceProviderIsService>();

            if (!IsValidatable(methodInfo, isService))
            {
                return;
            }

            builder.Metadata.Add(new ProducesResponseTypeMetadata(StatusCodes.Status400BadRequest, typeof(HttpValidationProblemDetails), ["application/problem+json"]));

            builder.FilterFactories.Add((context, next) =>
            {
                var validationDetails = new ValidationFilterDetails();
                foreach (var parameter in methodInfo.GetParameters())
                {
                    if (IsValidatable(parameter.ParameterType, isService))
                    {
                        validationDetails.Parameters.Add(parameter.ParameterType);
                    }
                    else
                    {
                        validationDetails.Parameters.Add(null);
                    }
                }

                if (logger.IsEnabled(LogLevel.Debug))
                {
                    logger.LogDebug("Validation filter will be added as route handler method '{methodName}' has validatable parameters.", context.MethodInfo.Name);
                }

                return async efic =>
                {
                    Debug.Assert(validationDetails is not null);
                    Debug.Assert(validationDetails.Parameters.Count == efic.Arguments.Count);

                    var useParameterValidationDetails = validationDetails is not null && validationDetails.Parameters.Count == efic.Arguments.Count;

                    for (var i = 0; i < validationDetails!.Parameters.Count; i++)
                    {
                        Type? parameterType = null;
                        if (useParameterValidationDetails)
                        {
                            parameterType = validationDetails!.Parameters[i];

                            if (parameterType is null)
                            {
                                continue;
                            }
                        }

                        var argument = efic.Arguments[i];

                        if (argument is null)
                        {
                            continue;
                        }

                        if (useParameterValidationDetails)
                        {
                            if (parameterType is not null && !parameterType.IsAssignableFrom(argument.GetType()))
                            {
                                continue;
                            }
                        }
                        else
                        {
                            if (!IsValidatable(argument.GetType()))
                            {
                                continue;
                            }
                        }

                        var context = new ValidationContext(instance: argument, serviceProvider: efic.HttpContext.RequestServices, items: null);
                        var results = new List<ValidationResult>();

                        if (!Validator.TryValidateObject(argument, context, results, validateAllProperties: true))
                        {
                            var errors = results
                                .GroupBy(r => r.MemberNames.FirstOrDefault() ?? string.Empty)
                                .ToDictionary(g => g.Key, g => g.Select(r => r.ErrorMessage ?? "").ToArray());

                            return TypedResults.ValidationProblem(errors);
                        }
                    }

                    return await next(efic);
                };
            });
        });

        return endpoint;
    }

    private static bool IsValidatable(MethodInfo methodInfo, IServiceProviderIsService? isService) =>
        methodInfo.GetParameters().Any(p => IsValidatable(p.ParameterType, isService));

    private static bool IsValidatable(Type type, IServiceProviderIsService? isService = null) =>
        !IsRequestDelegateFactorySpecialBoundType(type, isService)
        && typeof(IValidatableObject).IsAssignableFrom(type);

    private static bool IsRequestDelegateFactorySpecialBoundType(Type type, IServiceProviderIsService? isService) =>
        typeof(HttpContext) == type
        || typeof(HttpRequest) == type
        || typeof(HttpResponse) == type
        || typeof(ClaimsPrincipal) == type
        || typeof(CancellationToken) == type
        || typeof(IFormFileCollection) == type
        || typeof(IFormFile) == type
        || typeof(Stream) == type
        || typeof(PipeReader) == type
        || isService?.IsService(type) == true;
}

internal class ValidationFilterDetails
{
    public List<Type?> Parameters { get; } = [];
}
