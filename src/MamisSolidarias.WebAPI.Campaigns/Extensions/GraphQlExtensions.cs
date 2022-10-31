using HotChocolate.Diagnostics;
using MamisSolidarias.Infrastructure.Campaigns;
using MamisSolidarias.Utils.Security;
using MamisSolidarias.WebAPI.Campaigns.Queries;
using StackExchange.Redis;

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

        var redisConnectionString = $"{configuration["Redis:Host"]}:{configuration["Redis:Port"]}";
        services.AddSingleton(ConnectionMultiplexer.Connect(redisConnectionString));

        services.AddGraphQLServer()
            .AddQueryType<MochiQueries>()
            .AddInstrumentation(t =>
            {
                t.Scopes = ActivityScopes.All;
                t.IncludeDocument = true;
                t.RequestDetails = RequestDetails.All;
                t.IncludeDataLoaderKeys = true;
            })
            .AddAuthorization()
            .AddProjections()
            .AddFiltering()
            .AddSorting()
            .RegisterDbContext<CampaignsDbContext>()
            .InitializeOnStartup()
            .PublishSchemaDefinition(t => t
                .SetName($"{Services.Campaigns}gql")
                .AddTypeExtensionsFromFile("./Stitching.graphql")
                .PublishToRedis(configuration["GraphQl:GlobalSchemaName"],
                    sp => sp.GetRequiredService<ConnectionMultiplexer>()
                )
            );
        
    }
}