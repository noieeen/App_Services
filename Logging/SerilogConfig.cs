using Microsoft.Extensions.Configuration;
using Serilog;

namespace Logging;

public abstract class SerilogConfig
{
    public static LoggerConfiguration SerilogConfigDefaults(IConfiguration config)
    {
        var loggerConfig = new LoggerConfiguration()
            // .Filter.ByExcluding("RequestPath like '/health%'")
            // .Filter.ByExcluding("RequestPath like '/metrics%'")
            .ReadFrom.Configuration(config)
            .Enrich.FromLogContext();

        return loggerConfig;
    }
}