using FastEndpoints;
using MamisSolidarias.Infrastructure.Campaigns;
using MamisSolidarias.Infrastructure.Campaigns.Models.Base;

namespace MamisSolidarias.WebAPI.Campaigns.Endpoints.Campaigns.Juntos.Participants.Id.DELETE;

internal sealed class Endpoint : Endpoint<Request, Response>
{
    private readonly DbAccess _db;

    public Endpoint(CampaignsDbContext dbContext, DbAccess? dbAccess = null)
    {
        _db = dbAccess ?? new DbAccess(dbContext);
    }

    public override void Configure()
    {
        Delete("campaigns/juntos/participants/{id}");
        Policies(Utils.Security.Policies.CanWrite);
        Description(t=> t.WithTags("Juntos a la Par"));
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var participant = await _db.GetParticipant(req.Id, ct);
        if (participant is null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        participant.DonorId = null;
        participant.DonorName = null;
        participant.State = ParticipantState.MissingDonor;

        await _db.SaveChanges(ct);
        await SendOkAsync(new Response(participant.Id), ct);
    }


}