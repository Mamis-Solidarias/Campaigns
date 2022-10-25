using FastEndpoints;
using FastEndpoints.Swagger;
using MamisSolidarias.WebAPI.Campaigns.Extensions;

namespace MamisSolidarias.WebAPI.Campaigns.StartUp;

internal static class ServiceRegistrator
{
    public static void Register(WebApplicationBuilder builder)
    {

        builder.Services.AddDataProtection(builder.Configuration);
        builder.Services.AddOpenTelemetry(builder.Configuration,builder.Logging);
        builder.Services.AddFastEndpoints(t=> t.SourceGeneratorDiscoveredTypes = DiscoveredTypes.All);
        builder.Services.AddDbContext(builder.Configuration, builder.Environment);
        builder.Services.AddAuth(builder.Configuration);
        builder.Services.AddGraphQl(builder.Configuration);
        

        if (!builder.Environment.IsProduction())
            builder.Services.AddSwaggerDoc();
    }
}