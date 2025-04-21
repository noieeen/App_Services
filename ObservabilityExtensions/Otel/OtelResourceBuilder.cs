using Microsoft.Extensions.Hosting;
using OpenTelemetry.Resources;

namespace ObservabilityExtensions.Otel;

using OpenTelemetry.Resources;

public static class OtelResourceBuilder
{
    public static ResourceBuilder Create(string serviceName, string serviceVersion, Dictionary<string, object>? attrs = null)
    {
        return ResourceBuilder.CreateDefault()
            .AddService(serviceName: serviceName, serviceVersion: serviceVersion)
            .AddAttributes(attrs ?? new Dictionary<string, object>());
    }
}