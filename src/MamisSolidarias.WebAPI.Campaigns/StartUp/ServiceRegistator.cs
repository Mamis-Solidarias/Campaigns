using FastEndpoints;
using FastEndpoints.Security;
using FastEndpoints.Swagger;
using MamisSolidarias.Infrastructure.Campaigns;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace MamisSolidarias.WebAPI.Campaigns.StartUp;

internal static class ServiceRegistrator
{
    public static void Register(WebApplicationBuilder builder)
    {
        var connectionString = builder.Environment.EnvironmentName.ToLower() switch
        {
            "production" => builder.Configuration.GetConnectionString("Production"),
            _ => builder.Configuration.GetConnectionString("Development")
        };

        builder.Services.AddOpenTelemetryTracing(tracerProviderBuilder =>
        {
            tracerProviderBuilder
                .AddConsoleExporter()
                .AddJaegerExporter(t =>
                {
                    t.Endpoint = new Uri(
                        builder.Configuration["OpenTelemetry:Jaeger:Endpoint"]
                        ?? "http://localhost:14268/api/traces"
                    );
                })
                .AddSource(builder.Configuration["OpenTelemetry:Name"])
                .SetResourceBuilder(
                    ResourceBuilder.CreateDefault()
                        .AddService(
                            builder.Configuration["OpenTelemetry:Name"],
                            serviceVersion: builder.Configuration["OpenTelemetry:Version"]
                        )
                )
                .AddHttpClientInstrumentation(t =>
                {
                    t.RecordException = true;
                    t.SetHttpFlavor = true;
                })
                .AddAspNetCoreInstrumentation(t=> t.RecordException = true)
                .AddEntityFrameworkCoreInstrumentation(t=> t.SetDbStatementForText = true);
        });
        builder.Services.AddFastEndpoints();
        builder.Services.AddAuthenticationJWTBearer(builder.Configuration["JWT:Key"]);
        builder.Services.AddDbContext<CampaignsDbContext>(
            t =>
                t.UseNpgsql(connectionString, r => r.MigrationsAssembly("MamisSolidarias.WebAPI.Campaigns"))
                    .EnableSensitiveDataLogging(!builder.Environment.IsProduction())
        );

        if (!builder.Environment.IsProduction())
            builder.Services.AddSwaggerDoc();
    }
}