using FastEndpoints;

namespace MamisSolidarias.WebAPI.Campaigns.Endpoints.Campaigns.Mochi.Id.PUT;

internal sealed class Description : Summary<Endpoint>
{
    public Description()
    {
        Summary = "It updates a campaign edition";
        ExampleRequest = new Request
        {
            Id = 123,
            AddedBeneficiaries = new[] { 1, 2, 3 },
            RemovedBeneficiaries = new[] { 4, 5, 6 },
            Description = "Campaign description",
            Provider = "Provider name"
        };

        Response();
        Response(400, "Invalid request");
        Response(401, "Unauthorized");
        Response(403, "Forbidden");
        Response(404, "Campaign not found");
    }
}