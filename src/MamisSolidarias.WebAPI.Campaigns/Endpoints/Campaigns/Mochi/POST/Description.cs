using FastEndpoints;

namespace MamisSolidarias.WebAPI.Campaigns.Endpoints.Campaigns.Mochi.POST;

internal sealed class Description : Summary<Endpoint>
{
    public Description()
    {
        ExampleRequest = new Request
        {
            Edition = "2022",
            CommunityId = "TXT",
            Beneficiaries = new[] { 1, 2, 3 },
            Description = "Campaign Description",
            Provider = "Provider name"
        };

        Summary = "Created a new edition of the 'Una Mochi como la tuya' campaign";
        Response<Response>(201);
        Response(400, "Invalid request");
        Response(401, "Unauthorized");
        Response(403, "Forbidden");
        Response(409, "Conflict");
    }
}