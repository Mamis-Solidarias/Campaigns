using EntityFramework.Exceptions.PostgreSQL;
using MamisSolidarias.Infrastructure.Campaigns;
using Microsoft.EntityFrameworkCore;

namespace MamisSolidarias.WebAPI.Campaigns.Extensions;

internal static class EntityFrameworkExtensions
{
    public static void AddEntityFramework(this IServiceCollection services, IConfiguration configuration,
        IWebHostEnvironment environment, ILoggerFactory loggerFactory)
    {
        var logger = loggerFactory.CreateLogger("EntityFramework");
        var connectionString = configuration.GetConnectionString("CampaignsDb");

        if (string.IsNullOrWhiteSpace(connectionString)) logger.LogError("Connection string not found.");

        services.AddDbContext<CampaignsDbContext>(
            t =>
                t.UseNpgsql(connectionString, r => r.MigrationsAssembly("MamisSolidarias.WebAPI.Campaigns"))
                    .EnableSensitiveDataLogging(!environment.IsProduction())
                    .EnableDetailedErrors(!environment.IsProduction())
                    .UseExceptionProcessor()
                    .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
        );
    }

    public static void RunMigrations(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<CampaignsDbContext>();
        db.Database.Migrate();
    }
}