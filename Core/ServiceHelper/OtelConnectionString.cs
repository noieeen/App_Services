using Microsoft.Extensions.Configuration;

namespace Core.ServiceConnection;

public class OtelConnectionString
{
    public static string GetOtelConnectionString(IConfiguration configuration)
    {
        return configuration.GetConnectionString("OTLP_ENDPOINT_URL") ??
               throw new ArgumentNullException("OTLP_ENDPOINT_URL");
    }
}