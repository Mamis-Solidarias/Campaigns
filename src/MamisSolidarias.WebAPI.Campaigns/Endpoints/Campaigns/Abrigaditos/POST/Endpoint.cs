using EntityFramework.Exceptions.Common;
using FastEndpoints;
using MamisSolidarias.GraphQlClient;
using MamisSolidarias.Infrastructure.Campaigns;
using MamisSolidarias.Infrastructure.Campaigns.Models.Abrigaditos;
using MamisSolidarias.Infrastructure.Campaigns.Models.Base;
using MamisSolidarias.Messages;
using MamisSolidarias.WebAPI.Campaigns.Extensions;
using MassTransit;
using StrawberryShake;

namespace MamisSolidarias.WebAPI.Campaigns.Endpoints.Campaigns.Abrigaditos.POST;

internal sealed class Endpoint : Endpoint<Request, Response>
{
    private readonly IBus _bus;
    private readonly CampaignsDbContext _db;
    private readonly IGraphQlClient _graphQlClient;

    public Endpoint(CampaignsDbContext db, IGraphQlClient graphQlClient, IBus _bus)
    {
        _db = db;
        _graphQlClient = graphQlClient;
        this._bus = _bus;
    }

    public override void Configure()
    {
        Post("campaigns/abrigaditos");
        Policies(Utils.Security.Policies.CanWrite);
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var campaign = Map(req);

        var executor = await _graphQlClient
            .GetCommunity
            .ExecuteAsync(campaign.CommunityId, ct);
        var hasErrors = await executor.HandleErrors(
            async t => await SendForbiddenAsync(t),
            async (e, t) => await SendGraphQlErrors(e, t),
            ct);

        if (hasErrors)
            return;

        if (executor.Data?.Community?.Id is null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        try
        {
            await _db.AbrigaditosCampaigns.AddAsync(campaign, ct);
            await _db.SaveChangesAsync(ct);
            await _bus.PublishBatch(req.Beneficiaries.Select(t =>
                    new ParticipantAddedToAbrigaditosCampaign(campaign.Id, t)),
                ct);
            await SendAsync(new Response(campaign.Id), 201, ct);
        }
        catch (UniqueConstraintException)
        {
            AddError("Ya existe una campaña con esa comunidad y edición");
            await SendErrorsAsync(cancellation: ct);
        }
    }

    private static AbrigaditosCampaign Map(Request req)
    {
        return new AbrigaditosCampaign
        {
            Description = req.Description?.Trim(),
            Provider = req.Provider?.Trim(),
            Edition = req.Edition.Trim(),
            CommunityId = req.CommunityId,
            Participants = new List<AbrigaditosParticipant>(),
            Donations = new List<CampaignDonation>(),
            FundraiserGoal = req.FundraiserGoal
        };
    }

    private async Task SendGraphQlErrors(IEnumerable<IClientError> errors, CancellationToken token)
    {
        foreach (var clientError in errors)
            AddError(clientError.Message);

        await SendErrorsAsync(cancellation: token);
    }

}