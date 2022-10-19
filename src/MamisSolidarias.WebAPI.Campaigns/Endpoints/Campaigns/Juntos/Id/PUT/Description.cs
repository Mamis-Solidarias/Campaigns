using FastEndpoints;

namespace MamisSolidarias.WebAPI.Campaigns.Endpoints.Campaigns.Juntos.Id.PUT;

internal sealed class Description : Summary<Endpoint>
{
    public Description()
    {
        Summary = "It edits an edition of 'Juntos a la par'";
        ExampleRequest = new Request
        {
            AddedBeneficiaries = new[] { 1, 2, 3 },
            RemovedBeneficiaries = new[] { 4, 5, 6 },
            Description = "Description of the edition",
            Provider = "Provider of the edition",
            FundraiserGoal = 9999999,
            Id = 123
        };
        Response();
        Response(400, "Invalid request");
        Response(401, "Unauthorized");
        Response(403, "Forbidden");
        Response(404, "Campaign not found");
    }
}