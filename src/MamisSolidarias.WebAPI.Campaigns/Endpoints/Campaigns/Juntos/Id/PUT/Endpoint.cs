using FastEndpoints;
using MamisSolidarias.Infrastructure.Campaigns;
using MamisSolidarias.Messages;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace MamisSolidarias.WebAPI.Campaigns.Endpoints.Campaigns.Juntos.Id.PUT;

internal sealed class Endpoint : Endpoint<Request>
{
    private readonly IBus _bus;
    private readonly CampaignsDbContext _db;

    public Endpoint(CampaignsDbContext dbContext, IBus bus)
    {
        _bus = bus;
        _db = dbContext;
    }

    public override void Configure()
    {
        Put("campaigns/juntos/{Id}");
        Policies(Utils.Security.Policies.CanWrite);
        Description(t=> t.WithTags("Juntos a la Par"));
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        try
        {
            if (req.RemovedBeneficiaries.Any())
                await _db.JuntosParticipants
                    .Where(x => x.CampaignId == req.Id && req.RemovedBeneficiaries.Contains(x.BeneficiaryId))
                    .ExecuteDeleteAsync(ct);
            
            var campaign = await _db.JuntosCampaigns
                .AsTracking()
                .Include(t=> t.Participants)
                .SingleAsync(t=> t.Id == req.Id, ct);

            if (req.AddedBeneficiaries.Any())
            {
                await _bus.PublishBatch(req.AddedBeneficiaries
                    .Distinct()
                    .Where(id => campaign.Participants.All(p => p.BeneficiaryId != id))
                    .Select(id => new ParticipantAddedToJuntosCampaign(campaign.Id, id)),
                    ct
                );
            }
            
            campaign.Description = string.IsNullOrWhiteSpace(req.Description) ? null : req.Description.Trim();
            campaign.Provider = string.IsNullOrWhiteSpace(req.Provider) ? null : req.Provider.Trim();
            campaign.FundraiserGoal = req.FundraiserGoal;

            await _db.SaveChangesAsync(ct);
            await SendOkAsync(ct);
        }
        catch (InvalidOperationException)
        {
            await SendNotFoundAsync(ct);
        }
    }
}