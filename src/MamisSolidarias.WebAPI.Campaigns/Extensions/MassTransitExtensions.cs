using System.Reflection;
using MassTransit;

namespace MamisSolidarias.WebAPI.Campaigns.Extensions;

public static class MassTransitExtensions
{
    public static void AddMassTransit(this IServiceCollection services)
    {
        services.AddMassTransit(x =>
        {
            x.SetKebabCaseEndpointNameFormatter();
            x.AddConsumers(Assembly.GetExecutingAssembly());
            
            x.UsingRabbitMq((context, configurator) =>
            {
                var configuration = context.GetRequiredService<IConfiguration>();
                configurator.Host(configuration.GetConnectionString("RabbitMQ"));
                configurator.ConfigureEndpoints(context);
            });
        });
    }
}