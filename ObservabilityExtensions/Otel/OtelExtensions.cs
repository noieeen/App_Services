using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;


namespace ObservabilityExtensions.Otel;

public static class OtelExtensions
{
    public static IHostApplicationBuilder AddOtel(this IHostApplicationBuilder builder, ResourceBuilder resourceBuilder)
    {
        builder.Logging.ClearProviders();

        builder.Logging.AddOpenTelemetry(logging =>
        {
            if (builder.Environment.IsDevelopment())
                logging.AddConsoleExporter();

            logging.IncludeFormattedMessage = true;
            logging.IncludeScopes = true;
        });

        builder.Services.AddOpenTelemetry()
            .WithMetrics(metrics =>
            {
                metrics
                    .AddPrometheusExporter()
                    .AddProcessInstrumentation()
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation()
                    .AddMeter("Microsoft.AspNetCore.Hosting")
                    .AddMeter("Microsoft.AspNetCore.Server.Kestrel")
                    .AddMeter("System.Net.Http")
                    .AddMeter("System.Net.NameResolution")
                    // RabbitMQ
                    .AddMeter("RabbitMQ.Metrics");
            });

        builder.Services.AddOpenTelemetry()
            .WithTracing(tracing =>
            {
                tracing
                    .AddAspNetCoreInstrumentation(options =>
                    {
                        options.Filter = httpContext =>
                            !httpContext.Request.Path.StartsWithSegments("/metrics");
                    })
                    .AddGrpcClientInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddSqlClientInstrumentation()
                    .AddEntityFrameworkCoreInstrumentation()
                    .AddRedisInstrumentation()
                    .AddMassTransitInstrumentation();
            });

        builder.AddOpenTelemetryExporters();

        return builder;
    }

    private static IHostApplicationBuilder AddOpenTelemetryExporters(this IHostApplicationBuilder builder)
    {
        var otelConnectionString = builder.Configuration.GetConnectionString("OTLP_ENDPOINT_URL") ??
                                   throw new ArgumentNullException(
                                       "builder.Configuration.GetConnectionString(\"OTLP_ENDPOINT_URL\")");

        if (!string.IsNullOrWhiteSpace(otelConnectionString))
        {
            builder.Services.AddOpenTelemetry().WithMetrics(options => options.AddOtlpExporter(x =>
            {
                x.Endpoint = new Uri(otelConnectionString);
                x.Protocol = OtlpExportProtocol.Grpc;
            }));

            builder.Services.AddOpenTelemetry().WithTracing(options => options.AddOtlpExporter(x =>
            {
                x.Endpoint = new Uri(otelConnectionString);
                x.Protocol = OtlpExportProtocol.Grpc;
            }));

            // Optional OTLP log export (currently commented out)
            // builder.Logging.AddOpenTelemetry(options => options.AddOtlpExporter(x =>
            // {
            //     x.Endpoint = new Uri(otelConnectionString);
            //     x.Protocol = OtlpExportProtocol.Grpc;
            // }));
        }

        return builder;
    }
}