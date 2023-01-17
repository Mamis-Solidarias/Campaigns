using FastEndpoints;

namespace MamisSolidarias.WebAPI.Campaigns.Endpoints.Campaigns.Mochi.Id.POST;

internal sealed class Description : Summary<Endpoint>
{
    public Description()
    {
        Summary = "It imports an existing campaign into a new one.";
        ExampleRequest = new Request
        {
            CommunityId = "TXT",
            Edition = "2023",
            PreviousCampaignId = 123
        };

        Response<Response>(201);
        Response(400, "Invalid request");
        Response(401, "Unauthorized");
        Response(403, "Forbidden");
        Response(404, "Not found");
        Response(409, "Conflict");
    }
}