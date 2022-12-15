using FastEndpoints;
using FastEndpoints.Swagger;
using MamisSolidarias.WebAPI.Campaigns.Extensions;

namespace MamisSolidarias.WebAPI.Campaigns.StartUp;

internal static class ServiceRegistrar
{
    private static ILoggerFactory CreateLoggerFactory(IConfiguration configuration)
    {
        return LoggerFactory.Create(loggingBuilder => loggingBuilder
            .AddConfiguration(configuration)
            .AddConsole()
        );
    }

    public static void Register(WebApplicationBuilder builder)
    {
        using var loggerFactory = CreateLoggerFactory(builder.Configuration);

        builder.Services.AddDataProtection(builder.Configuration, loggerFactory);
        builder.Services.AddOpenTelemetry(builder.Configuration, builder.Logging, loggerFactory);
        builder.Services.AddFastEndpoints(t => t.SourceGeneratorDiscoveredTypes = DiscoveredTypes.All);
        builder.Services.AddEntityFramework(builder.Configuration, builder.Environment, loggerFactory);
        builder.Services.AddAuth(builder.Configuration, loggerFactory);
        builder.Services.AddGraphQl(builder.Configuration, loggerFactory);
        builder.Services.AddRedis(builder.Configuration, loggerFactory);
        builder.Services.AddSwaggerDoc();
    }
}