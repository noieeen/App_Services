using Microsoft.Extensions.Configuration;

namespace Core.ServiceHelper;

public abstract class OtelConnectionString
{
    public static string GetOtelGrpcConnectionString(IConfiguration configuration)
    {
        return configuration.GetConnectionString("OTLP_ENDPOINT_URL") ??
               throw new ArgumentNullException("OTLP_ENDPOINT_URL");
    }
    
    public static string GetOtelHttpConnectionString(IConfiguration configuration)
    {
        return configuration.GetConnectionString("OTLP_ENDPOINT_HTTP_URL") ??
               throw new ArgumentNullException("OTLP_ENDPOINT_HTTP_URL");
    }
}