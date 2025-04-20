using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Core.ExceptionHandlers;

public class DivideByZeroExceptionHandler(ILogger<DivideByZeroExceptionHandler> _logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        // if exception is not DivideByZeroException then skip exception handling
        if (exception is not DivideByZeroException zeroException)
        {
            return false;
        }

        _logger.LogError(exception, "DivideByZeroException caught: {Message}", exception.Message);

        var problemDetails = new ProblemDetails()
        {
            Title = "Divide by zero error",
            Status = StatusCodes.Status400BadRequest,
            Detail = exception.Message,
            Type = exception.GetType().Name,
            Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}"
        };

        httpContext.Response.StatusCode = problemDetails.Status.Value;

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}