using System.Net.Http;
using MamisSolidarias.HttpClient.Campaigns.Services;
using MamisSolidarias.Utils.Http;
using Microsoft.AspNetCore.Http;

namespace MamisSolidarias.HttpClient.Campaigns.CampaignsClient;

public partial class CampaignsClient : ICampaignsClient
{
    private readonly HeaderService _headerService;
    private readonly IHttpClientFactory _httpClientFactory;
    
    public CampaignsClient(IHttpContextAccessor? contextAccessor,IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
        _headerService = new HeaderService(contextAccessor);
    }
    
    private ReadyRequest CreateRequest(HttpMethod httpMethod,params string[] urlParams)
    {
        var client = _httpClientFactory.CreateClient("Campaigns");
        var request = new HttpRequestMessage(httpMethod, string.Join('/', urlParams));
        
        return new ReadyRequest(client,request);
    }
}