using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Logging;

public static class LoggingExtensions
{
    // public static void AddDefaultLogging(this IHostBuilder host)
    // {
    //     host.UseSerilog((ctx, cfg) => { cfg.ReadFrom.Configuration(ctx.Configuration); });
    // }
    public static IHostBuilder AddStructuredLogging(this IHostBuilder host)
    // public static IHostApplicationBuilder AddStructuredLogging(this IHostApplicationBuilder builder)
    {
        var otelConnectionString = builder.Configuration.GetConnectionString("OTLP_ENDPOINT_HTTP_URL") ??
                                   throw new ArgumentNullException(
                                       "builder.Configuration.GetConnectionString(\"OTLP_ENDPOINT_HTTP_URL\")");

        if (!string.IsNullOrWhiteSpace(otelConnectionString))
        {
            var loggerConfig = new LoggerConfiguration()
                // .Filter.ByExcluding("RequestPath like '/health%'")
                // .Filter.ByExcluding("RequestPath like '/metrics%'")
                .ReadFrom.Configuration(builder.Configuration)
                .Enrich.FromLogContext();


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