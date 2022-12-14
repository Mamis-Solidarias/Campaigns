using FastEndpoints;

namespace MamisSolidarias.WebAPI.Campaigns.Endpoints.Campaigns.Juntos.Id.DELETE;

internal sealed class Description : Summary<Endpoint>
{
    public Description()
    {
        Summary = "It edits an edition of 'Juntos a la par'";
        ExampleRequest = new Request
        {
            Id = 123
        };
        
        Response();
        Response(400, "Invalid request");
        Response(401, "Unauthorized");
        Response(403, "Forbidden");
        Response(404, "Campaign not found");
        Response(500, "Internal server error");
    }
}