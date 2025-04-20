using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Core.ExceptionHandlers;

public class ValidationExceptionHandler(ILogger<ValidationExceptionHandler> _logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not ValidationException validationException)
        {
            return false;
        }

        _logger.LogError(exception, "ValidationException caught: {Message}", exception.Message);
        var problemDetails = new ProblemDetails()
        {
            Title = "Validation failed",
            Status = StatusCodes.Status400BadRequest,
            Detail = "A validation error occurred.",
            Type = exception.GetType().Name,
            Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}",
            Extensions = { ["errorMessage"] = validationException.ValidationResult?.ErrorMessage ?? validationException.Message }
        };

        httpContext.Response.StatusCode = problemDetails.Status.Value;

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}