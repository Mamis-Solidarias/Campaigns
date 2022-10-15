using FastEndpoints;

namespace MamisSolidarias.WebAPI.Campaigns.Endpoints.Campaigns.Mochi.Id.DELETE;

internal sealed class Description : Summary<Endpoint>
{
    public Description()
    {
        Summary = "It deletes a campaign";
        ExampleRequest = new Request
        {
            Id = 123
        };
        Response();
        Response(400, "Invalid id");
        Response(401, "Unauthorized");
        Response(403, "Forbidden");
        Response(404, "Not found");
    }
}