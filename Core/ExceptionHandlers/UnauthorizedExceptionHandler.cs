using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Core.ExceptionHandlers;


public class UnauthorizedExceptionHandler(ILogger<UnauthorizedExceptionHandler> _logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not UnauthorizedAccessException unauthorizedException)
            return false;

        _logger.LogWarning(exception, "Unauthorized access attempt: {Message}", unauthorizedException.Message);

        var problemDetails = new ProblemDetails()
        {
            Title = "Unauthorized",
            Status = StatusCodes.Status401Unauthorized,
            Detail = unauthorizedException.Message,
            Type = exception.GetType().Name,
            Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}"
        };

        httpContext.Response.StatusCode = problemDetails.Status.Value;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}