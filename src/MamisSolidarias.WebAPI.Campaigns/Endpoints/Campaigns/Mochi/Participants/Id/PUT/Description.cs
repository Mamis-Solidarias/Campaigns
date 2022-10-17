using FastEndpoints;
using MamisSolidarias.Infrastructure.Campaigns.Models;

namespace MamisSolidarias.WebAPI.Campaigns.Endpoints.Campaigns.Mochi.Participants.Id.PUT;

internal sealed class Description : Summary<Endpoint>
{
    public Description()
    {
        Summary = "It assigns a donor to a beneficiary in a campaign.";
        ExampleRequest = new Request
        {
            DonorId = 123,
            DonationType = DonationType.Money.ToString(),
            DonationDropOffLocation = "La matanza",
            Id = 111
        };
        
        Response<Response>();
        Response(400, "Bad request");
        Response(401, "Unauthorized");
        Response(403, "Forbidden");
        Response(404, "Not found");
    }
}