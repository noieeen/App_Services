using System.Text.Json;
using Core.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace Middleware;

public class ApiResponseMiddleware
{
    private readonly RequestDelegate _next;

    public ApiResponseMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        var originalBody = context.Response.Body;

        using var memStream = new MemoryStream();
        context.Response.Body = memStream;

        await _next(context);

        memStream.Seek(0, SeekOrigin.Begin);
        var responseBody = new StreamReader(memStream).ReadToEnd();

        context.Response.Body = originalBody;

        // Only wrap if response is ProblemDetails
        if (context.Response.ContentType?.Contains("application/problem+json") == true)
        {
            var problem = JsonSerializer.Deserialize<ProblemDetails>(responseBody, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            var wrapped = new ApiResponse<object>
            {
                Success = false,
                Message = problem?.Detail ?? "An error occurred",
                Data = null
            };

            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(wrapped));
        }
        else
        {
            memStream.Seek(0, SeekOrigin.Begin);
            await memStream.CopyToAsync(originalBody);
        }
    }
}