using HotChocolate.Diagnostics;
using MamisSolidarias.Infrastructure.Campaigns;
using MamisSolidarias.Utils.Security;
using StackExchange.Redis;

namespace MamisSolidarias.WebAPI.Campaigns.Extensions;

internal static class GraphQlExtensions
{
    public static void AddGraphQl(this IServiceCollection services, IConfiguration configuration, ILoggerFactory loggerFactory)
    {
        var logger = loggerFactory.CreateLogger("GraphQL");
        var options = configuration.GetSection("GraphQL").Get<GraphQlOptions>();

        if (options is null)
        {
            logger.LogError("GraphQL configuration is missing");
            throw new ArgumentException("GraphQL configuration is missing");
        }
        
        services.AddGraphQlClient()
            .ConfigureHttpClient((s, t) =>
            {
                t.BaseAddress = new Uri(options.Endpoint);
                t.Timeout = TimeSpan.FromSeconds(10);

                var context = s.GetRequiredService<IHttpContextAccessor>().HttpContext;
                if (context is null)
                    return;

                if (context.Request.Headers.TryGetValue("Cookie", out var cookie) && cookie.Any())
                    t.DefaultRequestHeaders.Add("Cookie", cookie.First());
                if (context.Request.Headers.TryGetValue("Authorization", out var auth) && auth.Any())
                    t.DefaultRequestHeaders.Add("Authorization", auth.First());
            });

        services.AddGraphQLServer()
            .AddQueryType(t=> t.Name("Query"))
            .AddCampaignsTypes()
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
                .PublishToRedis(options.GlobalSchemaName,
                    sp => sp.GetRequiredService<ConnectionMultiplexer>()
                )
            );
    }
    private sealed record GraphQlOptions(string Endpoint, string GlobalSchemaName);
}