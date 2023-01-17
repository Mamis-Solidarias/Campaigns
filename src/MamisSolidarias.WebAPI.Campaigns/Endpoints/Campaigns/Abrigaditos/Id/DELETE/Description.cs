using FastEndpoints;

namespace MamisSolidarias.WebAPI.Campaigns.Endpoints.Campaigns.Abrigaditos.Id.Delete;

internal sealed class Description : Summary<Endpoint>
{
    public Description()
    {
        Summary = "It deletes an edition of 'Abrigaditos'";
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