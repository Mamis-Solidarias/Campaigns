using FastEndpoints;

namespace MamisSolidarias.WebAPI.Campaigns.Endpoints.Campaigns.Juntos.POST;

internal sealed class Description : Summary<Endpoint>
{
    public Description()
    {
        Summary = "It creates a new edition of the campaing 'Juntos a la par'";
        ExampleRequest = new Request
        {
            Beneficiaries = new[] {1, 2, 3},
            CommunityId = "TXT",
            Description = "Description of the edition",
            Edition = "2023",
            FundraiserGoal = 1000,
            Provider = "Shoes.com"
        };
        Response<Response>(201);
        Response(400, "Invalid request");
        Response(401, "Unauthorized");
        Response(403, "Forbidden");
        Response(409, "Conflict");
    }
}