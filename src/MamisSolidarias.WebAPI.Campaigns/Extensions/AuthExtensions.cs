using FastEndpoints.Security;
using MamisSolidarias.Utils.Security;

namespace MamisSolidarias.WebAPI.Campaigns.Extensions;

internal static class AuthExtensions
{
    public static void AddAuth(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthenticationJWTBearer(
            configuration["JWT:Key"],
            configuration["JWT:Issuer"]
        );
        services.AddAuthorization(t=> t.ConfigurePolicies(Services.Campaigns));
    }
}