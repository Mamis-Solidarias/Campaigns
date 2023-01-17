using FastEndpoints;
using MamisSolidarias.Infrastructure.Campaigns;
using MamisSolidarias.Infrastructure.Campaigns.Models.Base;
using Microsoft.EntityFrameworkCore;

namespace MamisSolidarias.WebAPI.Campaigns.Endpoints.Campaigns.Mochi.Participants.Id.DELETE;

internal sealed class Endpoint : Endpoint<Request, Response>
{
    private readonly CampaignsDbContext _db;

    public Endpoint(CampaignsDbContext dbContext)
    {
        _db = dbContext;
    }

    public override void Configure()
    {
        Delete("campaigns/mochi/participants/{id}");
        Policies(Utils.Security.Policies.CanWrite);
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        try
        {
            var participant = await _db.MochiParticipants
                .AsTracking()
                .SingleAsync(t => t.Id == req.Id, ct);

            participant.DonorId = null;
            participant.DonorName = null;
            participant.DonationDropOffPoint = null;
            participant.DonationType = null;
            participant.State = ParticipantState.MissingDonor;

            await _db.SaveChangesAsync(ct);
            await SendOkAsync(new Response(participant.Id), ct);
        }
        catch (InvalidOperationException)
        {
            await SendNotFoundAsync(ct);
        }
    }
}