using MamisSolidarias.Utils.Http;

namespace MamisSolidarias.HttpClient.Campaigns.CampaignsClient;

public class CampaignsClient : ICampaignsClient
{
    private readonly IHttpClientFactory _httpClientFactory;

    public CampaignsClient(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    private ReadyRequest CreateRequest(HttpMethod httpMethod, params string[] urlParams)
    {
        var client = _httpClientFactory.CreateClient("Campaigns");
        var request = new HttpRequestMessage(httpMethod, string.Join('/', urlParams));

        return new ReadyRequest(client, request);
    }
}