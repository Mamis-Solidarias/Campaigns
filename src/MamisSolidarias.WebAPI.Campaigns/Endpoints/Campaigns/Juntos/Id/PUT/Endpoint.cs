using FastEndpoints;
using MamisSolidarias.Infrastructure.Campaigns;
using MamisSolidarias.Messages;
using MassTransit;
using StrawberryShake;

namespace MamisSolidarias.WebAPI.Campaigns.Endpoints.Campaigns.Juntos.Id.PUT;

internal sealed class Endpoint : Endpoint<Request>
{
    private readonly IBus _bus;
    private readonly DbAccess _db;

    public Endpoint(IBus bus, CampaignsDbContext dbContext, DbAccess? dbAccess = null)
    {
        _bus = bus;
        _db = dbAccess ?? new DbAccess(dbContext);
    }

    public override void Configure()
    {
        Put("campaigns/juntos/{Id}");
        Policies(Utils.Security.Policies.CanWrite);
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var campaign = await _db.GetCampaign(req.Id, ct);
        if (campaign is null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        if (req.RemovedBeneficiaries.Any())
            await _db.DeleteParticipants(req.Id, req.RemovedBeneficiaries, ct);

        var idList = req.AddedBeneficiaries
            .Distinct()
            .Where(id => campaign.Participants.All(p => p.BeneficiaryId != id));

        foreach (var id in idList)
        {
            await _bus.Publish<ParticipantAddedToJuntosCampaign>(
                new(campaign.Id, id),
                ct
            );
        }

        campaign.Description = string.IsNullOrWhiteSpace(req.Description) ? null : req.Description.Trim();
        campaign.Provider = string.IsNullOrWhiteSpace(req.Provider) ? null : req.Provider.Trim();
        campaign.FundraiserGoal = req.FundraiserGoal;

        await _db.SaveChanges(ct);
        await SendOkAsync(ct);
    }

    private async Task SendGraphQlErrors(IEnumerable<IClientError> errors, CancellationToken token)
    {
        foreach (var clientError in errors)
            AddError(clientError.Message);

        await SendErrorsAsync(cancellation: token);
    }
}
