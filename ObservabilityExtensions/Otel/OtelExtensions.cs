using Microsoft.Extensions.DependencyInjection;

namespace Opentelemetry;

public static class OtelExtensions
{
    public static IServiceCollection AddOtel(this IServiceCollection services)
    {
        // services.AddOpenTelemetry()
        //     .WithTracing(builder => builder
        //         .AddAspNetCoreInstrumentation()
        //         .AddHttpClientInstrumentation()
        //         .SetResourceBuilder(OtelResourceBuilder.Create("MyApp"))
        //         .AddOtlpExporter());

        return services;
    }
}