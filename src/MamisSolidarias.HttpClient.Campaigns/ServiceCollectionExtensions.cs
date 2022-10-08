using System;
using MamisSolidarias.HttpClient.Campaigns.Models;
using MamisSolidarias.HttpClient.Campaigns.Services;
using MamisSolidarias.HttpClient.Campaigns.CampaignsClient;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;

namespace MamisSolidarias.HttpClient.Campaigns;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// It registers the CampaignsHttpClient using dependency injection
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    public static void AddCampaignsHttpClient(this IServiceCollection services, IConfiguration configuration)
    {
        var config = new CampaignsConfiguration();
        configuration.GetSection("CampaignsHttpClient").Bind(config);
        ArgumentNullException.ThrowIfNull(config.BaseUrl);
        ArgumentNullException.ThrowIfNull(config.Timeout);
        ArgumentNullException.ThrowIfNull(config.Retries);

        services.AddHttpContextAccessor();
        services.AddSingleton<ICampaignsClient, CampaignsClient.CampaignsClient>();
        services.AddHttpClient("Campaigns", (services,client) =>
        {
            client.BaseAddress = new Uri(config.BaseUrl);
            client.Timeout = TimeSpan.FromMilliseconds(config.Timeout);
            
            var contextAccessor = services.GetService<IHttpContextAccessor>();
            if (contextAccessor is not null)
            {
                var authHeader = new HeaderService(contextAccessor).GetAuthorization();
                if (authHeader is not null)
                    client.DefaultRequestHeaders.Add("Authorization", authHeader);
            }
            
        })
            .AddTransientHttpErrorPolicy(t =>
            t.WaitAndRetryAsync(config.Retries,
                retryAttempt => TimeSpan.FromMilliseconds(100 * Math.Pow(2, retryAttempt)))
        );
    }
}