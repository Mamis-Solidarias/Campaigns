using EntityFramework.Exceptions.PostgreSQL;
using MamisSolidarias.Infrastructure.Campaigns;
using Microsoft.EntityFrameworkCore;

namespace MamisSolidarias.WebAPI.Campaigns.Extensions;

internal static class DbContextExtensions
{
    public static void AddDbContext(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {
        var connectionString = environment.EnvironmentName.ToLower() switch
        {
            "production" => configuration.GetConnectionString("Production"),
            _ => configuration.GetConnectionString("Development")
        };
        
        services.AddDbContext<CampaignsDbContext>(
            t =>
                t.UseNpgsql(connectionString, r => r.MigrationsAssembly("MamisSolidarias.WebAPI.Campaigns"))
                    .EnableSensitiveDataLogging(!environment.IsProduction())
                    .EnableDetailedErrors(!environment.IsProduction())
                    .UseExceptionProcessor()
        );

    }
}