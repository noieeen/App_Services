using Core.ServiceHelper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Sinks.OpenTelemetry;

namespace Logging;

public static class LoggingServiceExtensions
{
    public static IServiceCollection AddDefaultLogging(this IServiceCollection services)
    {
        services.AddLogging();
        return services;
    }
    public static IHostApplicationBuilder AddStructuredLogging(this IHostApplicationBuilder builder)
    {
        var otelConnectionString = OtelConnectionString.GetOtelGrpcConnectionString(builder.Configuration);

        if (!string.IsNullOrWhiteSpace(otelConnectionString))
        {
            builder.Logging.ClearProviders();
            var loggerConfig = SerilogConfig.SerilogConfigDefaults(builder.Configuration);

            loggerConfig.WriteTo.OpenTelemetry(options =>
            {
                options.Endpoint = otelConnectionString;
                options.Protocol = OtlpProtocol.HttpProtobuf;
            });

            if (builder.Environment.IsDevelopment())
            {
                loggerConfig.WriteTo.Console();
            }

            Log.Logger = loggerConfig.CreateLogger();

            builder.Logging.ClearProviders();

            if (builder.Environment.IsDevelopment())
            {
                builder.Logging.AddConsole();
            }

            builder.Logging.AddSerilog(Log.Logger, dispose: true);
        }
        else if (builder.Environment.IsDevelopment())
        {
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
        }

        return builder;
    }
}