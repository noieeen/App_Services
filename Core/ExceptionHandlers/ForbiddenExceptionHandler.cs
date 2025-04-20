using Core.Exceptions;

namespace Core.ExceptionHandlers;

using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

public class ForbiddenExceptionHandler(ILogger<ForbiddenExceptionHandler> _logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not ForbiddenException forbiddenException)
            return false;

        _logger.LogWarning(exception, "Forbidden access attempt: {Message}", forbiddenException.Message);

        var problemDetails = new ProblemDetails()
        {
            Title = "Forbidden",
            Status = StatusCodes.Status403Forbidden,
            Detail = forbiddenException.Message,
            Type = exception.GetType().Name,
            Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}"
        };

        httpContext.Response.StatusCode = problemDetails.Status.Value;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}