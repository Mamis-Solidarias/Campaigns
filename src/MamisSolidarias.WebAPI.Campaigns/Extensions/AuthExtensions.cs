using FastEndpoints.Security;
using MamisSolidarias.Utils.Security;

namespace MamisSolidarias.WebAPI.Campaigns.Extensions;

internal static class AuthExtensions
{
    public static void AddAuth(this IServiceCollection services, IConfiguration configuration,ILoggerFactory loggerFactory)
    {
        var logger = loggerFactory.CreateLogger("Auth");
        var options = configuration.GetSection("JWT").Get<JwtOptions>();

        if (options is null)
        {
            logger.LogError("JWT options not found");
            throw new ArgumentException("JWT options not found");
        }
        
        services.AddAuthenticationJWTBearer(options.Key, options.Issuer);
        services.AddAuthorization(t=> t.ConfigurePolicies(Services.Campaigns));
    }
    
    private sealed record JwtOptions(string Key, string Issuer);
}