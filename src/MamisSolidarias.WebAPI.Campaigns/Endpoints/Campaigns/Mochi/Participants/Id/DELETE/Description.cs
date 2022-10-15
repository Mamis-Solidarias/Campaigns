using FastEndpoints;

namespace MamisSolidarias.WebAPI.Campaigns.Endpoints.Campaigns.Mochi.Participants.Id.DELETE;

internal sealed class Description : Summary<Endpoint>
{
    public Description()
    {
        Summary = "It removes the association between the participant and the donor";
        ExampleRequest = new Request
        {
            Id = 123
        };
        
        Response(200, "The donor was removed successfully",example: new Response(99));
        Response(400, "The participant id is invalid");
        Response(404, "The participant was not found");
        Response(401, "The user is not authorized to perform this action");
        Response(403, "The user is not allowed to perform this action");
    }
}