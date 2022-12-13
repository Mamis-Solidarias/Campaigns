using FastEndpoints;
using FastEndpoints.Swagger;
using MamisSolidarias.WebAPI.Campaigns.Extensions;

namespace MamisSolidarias.WebAPI.Campaigns.StartUp;

internal static class MiddlewareRegistrar
{
    public static void Register(WebApplication app)
    {
        app.UseDefaultExceptionHandler();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseFastEndpoints();
        app.MapGraphQL();

        app.RunMigrations();

        if (!app.Environment.IsProduction())
        {
            app.UseOpenApi();
            app.UseSwaggerUi3(t => t.ConfigureDefaults());
        }
    }
}