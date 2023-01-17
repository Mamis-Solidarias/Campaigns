using FastEndpoints;
using MamisSolidarias.Infrastructure.Campaigns;
using MamisSolidarias.Messages;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace MamisSolidarias.WebAPI.Campaigns.Endpoints.Campaigns.Mochi.Id.PUT;

internal sealed class Endpoint : Endpoint<Request>
{
    private readonly IBus _bus;
    private readonly CampaignsDbContext _db;

    public Endpoint(CampaignsDbContext dbContext, IBus bus)
    {
        _db = dbContext;
        _bus = bus;
    }

    public override void Configure()
    {
        Put("campaigns/mochi/{id}");
        Policies(Utils.Security.Policies.CanWrite);
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        try
        {
            if (req.RemovedBeneficiaries.Any())
                await _db.MochiParticipants
                    .Where(t => req.RemovedBeneficiaries.Contains(t.BeneficiaryId))
                    .Where(t => t.CampaignId == req.Id)
                    .ExecuteDeleteAsync(ct);
            
            var campaign = await _db.MochiCampaigns
                .AsTracking()
                .Include(t => t.Participants)
                .SingleAsync(t => t.Id == req.Id, ct);

            campaign.Description = string.IsNullOrWhiteSpace(req.Description) ? null : req.Description.Trim();
            campaign.Provider = string.IsNullOrWhiteSpace(req.Provider) ? null : req.Provider.Trim();

            await _db.SaveChangesAsync(ct);

            if (req.AddedBeneficiaries.Any())
            {
                var messages = req.AddedBeneficiaries
                    .Distinct()
                    .Where(t => campaign.Participants.All(r => r.BeneficiaryId != t))
                    .Select(t => new ParticipantAddedToMochiCampaign(campaign.Id, t));
                await _bus.PublishBatch(messages, ct);
            }
            
            await SendOkAsync(ct);
        }
        catch (InvalidOperationException)
        {
            await SendNotFoundAsync(ct);
        }
    }
}