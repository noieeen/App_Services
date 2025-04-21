using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Middleware;
using ObservabilityExtensions.Otel;

namespace ApiSetup;

public static class DefaultMiddlewareExtensions
{
    public static WebApplication UseDefaultApiPipeline(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        // app.UseRouting();
        app.UseExceptionHandler();
        app.UseAuthorization();
        app.UseMiddleware<ApiResponseMiddleware>();
        app.HealthCheckEndpoints();
        app.MapControllers();

        return app;
    }
}