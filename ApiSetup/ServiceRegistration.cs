using Core.ExceptionHandlers;
using Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using ObservabilityExtensions.Otel;

namespace ApiSetup;

public static class DefaultApiServiceExtensions
{
    public static IHostApplicationBuilder AddDefaultApi(this IHostApplicationBuilder builder, string serviceName,
        string serviceVersion, Dictionary<string, object>? attrs = null)
    {
        var resourceBuilder = OtelResourceBuilder.Create(serviceName, serviceVersion, attrs);

        builder.AddOtel(resourceBuilder);

        builder.Services.AddDefaultApiServices(builder.Configuration);

        // services.AddDefaultLogging();
        // services.AddMessaging();         // Reuse MyApp.Messaging
        // services.AddCustomLogging();     // Optional: logging setup
        // builder.AddMessaging();
        builder.Services.AddHealthChecks();
        builder.Services.AddDefaultLogging();

        return builder;
    }

    public static IHostApplicationBuilder AddDefaultHealthChecks(this IHostApplicationBuilder builder)
    {
        builder.Services.AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy(), ["live"]);

        return builder;
    }

    public static IServiceCollection AddDefaultApiServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddProblemDetails();

        services.AddExceptionHandler<ValidationExceptionHandler>();
        services.AddExceptionHandler<DivideByZeroExceptionHandler>();
        services.AddExceptionHandler<UnauthorizedExceptionHandler>();
        services.AddExceptionHandler<NotFoundExceptionHandler>();
        services.AddExceptionHandler<ForbiddenExceptionHandler>();
        services.AddExceptionHandler<GlobalExceptionHandler>(); // **Do last order, Handle other exception cases

        // services.AddDefaultLogging();
        // services.AddMessaging();         // Reuse MyApp.Messaging
        // services.AddCustomLogging();     // Optional: logging setup


        return services;
    }

    // public static IServiceCollection AddDefaultObservability(this IServiceCollection services, string serviceName)
    // {
    //     services.AddOpenTelemetry()
    //         .WithTracing(tracer =>
    //         {
    //             tracer
    //                 .AddAspNetCoreInstrumentation()
    //                 .AddHttpClientInstrumentation()
    //                 .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceName))
    //                 .AddOtlpExporter(); // Send to ClickHouse via OTLP Collector
    //         });
    //
    //     return services;
    // }
}