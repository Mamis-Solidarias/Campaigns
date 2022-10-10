namespace MamisSolidarias.WebAPI.Campaigns.Extensions;

internal static class GraphQlExtensions
{
    public static void AddGraphQl(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddGraphQlClient()
            .ConfigureHttpClient((s, t) =>
            {
                t.BaseAddress = new Uri(configuration["GraphQL:Endpoint"]);
                t.Timeout = TimeSpan.FromSeconds(10);

                var context = s.GetRequiredService<IHttpContextAccessor>().HttpContext;
                if (context is null)
                    return;

                if (context.Request.Headers.TryGetValue("Cookie", out var cookie) && cookie.Any())
                    t.DefaultRequestHeaders.Add("Cookie", cookie.First());
                if (context.Request.Headers.TryGetValue("Authorization", out var auth) && auth.Any())
                    t.DefaultRequestHeaders.Add("Authorization", auth.First());

            });
    }
}