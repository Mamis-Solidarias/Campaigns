using Npgsql;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace MamisSolidarias.WebAPI.Campaigns.Extensions;

internal static class OpenTelemetryExtensions
{
    public static void AddOpenTelemetry(this IServiceCollection services, IConfiguration configuration,
        ILoggingBuilder logging)
    {
        var options = configuration.GetSection("OpenTelemetry").Get<OpenTelemetryOptions>();

        var resourceBuilder = ResourceBuilder
            .CreateDefault()
            .AddService(options.Name, "MamisSolidarias", options.Version)
            .AddTelemetrySdk();

        services.AddOpenTelemetryTracing(tracerProviderBuilder =>
        {
            tracerProviderBuilder
                .SetResourceBuilder(resourceBuilder)
                .AddNewRelicExporter(options.NewRelic)
                .AddConsoleExporter(options.UseConsole)
                .AddJaegerExporter(options.Jaeger)
                .AddHttpClientInstrumentation(t => t.RecordException = true)
                .AddAspNetCoreInstrumentation(t => t.RecordException = true)
                .AddEntityFrameworkCoreInstrumentation(t => t.SetDbStatementForText = true)
                .AddHotChocolateInstrumentation()
                .AddNpgsql();
        });

        services.AddOpenTelemetryMetrics(meterProviderBuilder =>
        {
            meterProviderBuilder
                .SetResourceBuilder(resourceBuilder)
                .AddNewRelicExporter(options.NewRelic)
                .AddRuntimeInstrumentation()
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation();
        });

        // Configure the OpenTelemetry SDK for logs
        logging.ClearProviders();
        logging.AddConsole();

        logging.AddOpenTelemetry(logsProviderBuilder =>
        {
            logsProviderBuilder.IncludeFormattedMessage = true;
            logsProviderBuilder.ParseStateValues = true;
            logsProviderBuilder.IncludeScopes = true;
            logsProviderBuilder
                .SetResourceBuilder(resourceBuilder)
                .AddNewRelicExporter(options.NewRelic);
        });
    }

    private static TracerProviderBuilder AddNewRelicExporter(this TracerProviderBuilder builder,
        NewRelicOptions? newRelicOptions)
    {
        if (string.IsNullOrWhiteSpace(newRelicOptions?.Url) || string.IsNullOrWhiteSpace(newRelicOptions.ApiKey))
            return builder;

        return builder.AddOtlpExporter(t =>
        {
            t.Endpoint = new Uri(newRelicOptions.Url);
            t.Headers = $"api-key={newRelicOptions.ApiKey}";
        });
    }

    private static OpenTelemetryLoggerOptions AddNewRelicExporter(this OpenTelemetryLoggerOptions builder,
        NewRelicOptions? newRelicOptions)
    {
        if (string.IsNullOrWhiteSpace(newRelicOptions?.Url) || string.IsNullOrWhiteSpace(newRelicOptions.ApiKey))
            return builder;

        return builder.AddOtlpExporter(t =>
        {
            t.Endpoint = new Uri(newRelicOptions.Url);
            t.Headers = $"api-key={newRelicOptions.ApiKey}";
        });
    }

    private static MeterProviderBuilder AddNewRelicExporter(this MeterProviderBuilder builder,
        NewRelicOptions? newRelicOptions)
    {
        if (string.IsNullOrWhiteSpace(newRelicOptions?.Url) || string.IsNullOrWhiteSpace(newRelicOptions.ApiKey))
            return builder;

        return builder.AddOtlpExporter((t, m) =>
        {
            t.Endpoint = new Uri(newRelicOptions.Url);
            t.Headers = $"api-key={newRelicOptions.ApiKey}";
            m.TemporalityPreference = MetricReaderTemporalityPreference.Delta;
        });
    }

    private static TracerProviderBuilder AddJaegerExporter(this TracerProviderBuilder builder,
        JaegerOptions? jaegerOptions)
    {
        if (jaegerOptions is null || string.IsNullOrWhiteSpace(jaegerOptions.Url))
            return builder;

        return builder.AddJaegerExporter(t => t.AgentHost = jaegerOptions.Url);
    }

    private static TracerProviderBuilder AddConsoleExporter(this TracerProviderBuilder builder, bool useConsole)
    {
        if (useConsole)
            builder.AddConsoleExporter();
        return builder;
    }

    // ReSharper disable once ClassNeverInstantiated.Local
    private sealed class NewRelicOptions
    {
        // ReSharper disable file UnusedAutoPropertyAccessor.Local
        public string? ApiKey { get; init; }
        public string? Url { get; init; }
    }

    // ReSharper disable once ClassNeverInstantiated.Local
    private sealed class JaegerOptions
    {
        public string? Url { get; init; }
    }

    private sealed class OpenTelemetryOptions
    {
        public string Name { get; init; } = string.Empty;
        public string Version { get; init; } = string.Empty;
        public JaegerOptions? Jaeger { get; init; }
        public NewRelicOptions? NewRelic { get; init; }
        public bool UseConsole { get; init; }
    }
}