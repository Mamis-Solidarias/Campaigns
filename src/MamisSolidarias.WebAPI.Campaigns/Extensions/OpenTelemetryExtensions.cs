using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using YamlDotNet.Serialization.NodeTypeResolvers;

namespace MamisSolidarias.WebAPI.Campaigns.Extensions;

internal static class OpenTelemetryExtensions
{
    public static void AddOpenTelemetry(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOpenTelemetryTracing(tracerProviderBuilder =>
        {
            tracerProviderBuilder
                .AddConsoleExporter()
                .AddJaegerExporter(t =>
                {
                    var jaegerHost = configuration["OpenTelemetry:Jaeger:Host"];
                    if (jaegerHost is not null)
                        t.AgentHost = jaegerHost;
                    
                })
                .AddSource(configuration["OpenTelemetry:Name"])
                .SetResourceBuilder(
                    ResourceBuilder.CreateDefault()
                        .AddService(
                            configuration["OpenTelemetry:Name"],
                            serviceVersion: configuration["OpenTelemetry:Version"]
                        )
                )
                .AddHttpClientInstrumentation(t =>
                {
                    t.RecordException = true;
                    t.SetHttpFlavor = true;
                })
                .AddAspNetCoreInstrumentation(t => t.RecordException = true)
                .AddEntityFrameworkCoreInstrumentation(t => t.SetDbStatementForText = true)
                .AddHotChocolateInstrumentation()
                ;
        });
    }
}