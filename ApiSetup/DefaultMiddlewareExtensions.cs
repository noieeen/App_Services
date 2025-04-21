using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

namespace Middleware;

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
        app.UseRouting();
        app.UseExceptionHandler();
        app.UseAuthorization();
        app.UseMiddleware<ApiResponseMiddleware>();
        app.MapControllers();

        return app;
    }
}