using StackExchange.Redis;

namespace MamisSolidarias.WebAPI.Campaigns.Extensions;

internal static class RedisExtensions
{
    public static void AddRedis(this IServiceCollection services, IConfiguration configuration,
        ILoggerFactory loggerFactory)
    {
        var logger = loggerFactory.CreateLogger("Redis");
        var connectionString = configuration.GetConnectionString("Redis");

        if (configuration is null)
        {
            logger.LogError("Redis connection string is null");
            throw new ArgumentException("Redis connection string is null");
        }


        var redis = ConnectionMultiplexer.Connect(connectionString);

        if (redis is null)
        {
            logger.LogError("Could not connect to Redis");
            throw new ArgumentException("Could not connect to Redis");
        }

        services.AddSingleton(redis);
    }
}