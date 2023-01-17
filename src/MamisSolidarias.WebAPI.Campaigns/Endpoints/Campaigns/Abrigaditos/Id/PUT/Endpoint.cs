using FastEndpoints;
using MamisSolidarias.Infrastructure.Campaigns;
using MamisSolidarias.Messages;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace MamisSolidarias.WebAPI.Campaigns.Endpoints.Campaigns.Abrigaditos.Id.PUT;

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
        Put("campaigns/abrigaditos/{id}");
        Policies(Utils.Security.Policies.CanWrite);
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        try
        {
            if (req.RemovedBeneficiaries.Any())
                await _db.AbrigaditosParticipants
                    .Where(t => req.RemovedBeneficiaries.Contains(t.BeneficiaryId))
                    .Where(t => t.CampaignId == req.Id)
                    .ExecuteDeleteAsync(ct);

            var campaign = await _db.AbrigaditosCampaigns
                .AsTracking()
                .Include(t => t.Participants)
                .SingleAsync(t => t.Id == req.Id, ct);

            campaign.Description = req.Description;
            campaign.Provider = req.Provider;
            campaign.FundraiserGoal = req.FundraiserGoal;

            if (req.AddedBeneficiaries.Any())
            {
                var messages = req.AddedBeneficiaries
                    .Distinct()
                    .Where(t => campaign.Participants.All(r => r.BeneficiaryId != t))
                    .Select(t => new ParticipantAddedToAbrigaditosCampaign(campaign.Id, t));
                await _bus.PublishBatch(messages, ct);
            }

            await _db.SaveChangesAsync(ct);
            await SendOkAsync(ct);
        }
        catch (InvalidOperationException)
        {
            await SendNotFoundAsync(ct);
        }
    }
}