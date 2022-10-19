using FastEndpoints;
using MamisSolidarias.Infrastructure.Campaigns;
using MamisSolidarias.Infrastructure.Campaigns.Models;

namespace MamisSolidarias.WebAPI.Campaigns.Endpoints.Campaigns.Mochi.Participants.Id.DELETE;

internal sealed class Endpoint : Endpoint<Request, Response>
{
    private readonly DbAccess _db;

    public Endpoint(CampaignsDbContext dbContext, DbAccess? dbAccess = null)
    {
        _db = dbAccess ?? new DbAccess(dbContext);
    }

    public override void Configure()
    {
        Delete("campaigns/mochi/participants/{id}");
        Policies(Utils.Security.Policies.CanWrite);
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var participant = await _db.GetParticipantAsync(req.Id, ct);
        if (participant is null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        participant.DonorId = null;
        participant.DonorName = null;
        participant.DonationDropOffLocation = null;
        participant.DonationType = null;
        participant.State = ParticipantState.MissingDonor;

        await _db.UpdateParticipantAsync(participant, ct);
        await SendOkAsync(new Response(participant.Id), ct);
    }


}