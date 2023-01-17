using FastEndpoints;
using MamisSolidarias.Infrastructure.Campaigns;
using MamisSolidarias.Messages;
using MassTransit;

namespace MamisSolidarias.WebAPI.Campaigns.Endpoints.Campaigns.Mochi.Id.PUT;

internal sealed class Endpoint : Endpoint<Request>
{
    private readonly IBus _bus;
    private readonly DbAccess _db;

    public Endpoint(IBus bus, CampaignsDbContext dbContext, DbAccess? dbAccess = null)
    {
        _db = dbAccess ?? new DbAccess(dbContext);
        _bus = bus;
    }

    public override void Configure()
    {
        Put("campaigns/mochi/{id}");
        Policies(Utils.Security.Policies.CanWrite);
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var campaign = await _db.GetMochiAsync(req.Id, ct);
        if (campaign is null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        if (req.RemovedBeneficiaries.Any())
            await _db.DeleteParticipantsAsync(req.RemovedBeneficiaries, ct);

        campaign.Description = string.IsNullOrWhiteSpace(req.Description) ? null : req.Description.Trim();
        campaign.Provider = string.IsNullOrWhiteSpace(req.Provider) ? null : req.Provider.Trim();

        await _db.SaveChangesAsync(ct);

        var idList = req.AddedBeneficiaries
            .Distinct()
            .Where(id => campaign.Participants.All(p => p.BeneficiaryId != id));

        foreach (var id in idList)
            await _bus.Publish(
                new ParticipantAddedToMochiCampaign(id, campaign.Id),
                ct
            );

        await SendOkAsync(ct);
    }
}