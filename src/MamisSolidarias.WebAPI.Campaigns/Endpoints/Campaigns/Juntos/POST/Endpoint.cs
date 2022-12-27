using EntityFramework.Exceptions.Common;
using FastEndpoints;
using MamisSolidarias.GraphQlClient;
using MamisSolidarias.Infrastructure.Campaigns;
using MamisSolidarias.Infrastructure.Campaigns.Models;
using MamisSolidarias.Messages;
using MamisSolidarias.WebAPI.Campaigns.Extensions;
using MassTransit;
using StrawberryShake;

namespace MamisSolidarias.WebAPI.Campaigns.Endpoints.Campaigns.Juntos.POST;

internal sealed class Endpoint : Endpoint<Request, Response>
{
    private readonly DbAccess _db;
    private readonly IBus _bus;
    private readonly IGraphQlClient _graphQlClient;

    public Endpoint(IBus bus, IGraphQlClient graphQlClient, CampaignsDbContext dbContext, DbAccess? dbAccess = null)
    {
        _graphQlClient = graphQlClient;
        _bus = bus;
        _db = dbAccess ?? new DbAccess(dbContext);
    }

    public override void Configure()
    {
        Post("campaigns/juntos");
        Policies(Utils.Security.Policies.CanWrite);
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var campaign = new JuntosCampaign
        {
            Description = req.Description?.Trim(),
            CommunityId = req.CommunityId.Trim(),
            Edition = req.Edition.Trim(),
            FundraiserGoal = req.FundraiserGoal,
            Participants = new List<JuntosParticipant>(),
            Provider = req.Provider?.Trim()
        };

        var communityExecutor = await _graphQlClient.GetCommunity.ExecuteAsync(campaign.CommunityId, ct);

        var hasErrors = await communityExecutor.HandleErrors(
            async t => await SendForbiddenAsync(t),
            async (e, t) => await SendGraphQlErrors(e, t),
            ct
        );

        if (hasErrors)
            return;

        if (communityExecutor.Data?.Community is null)
        {
            await SendNotFoundAsync(ct);
            return;
        }
        
        try
        {
            await _db.AddCampaign(campaign, ct);
            foreach (var beneficiaryId in req.Beneficiaries.Distinct())
            {
                await _bus.Publish<ParticipantAddedToJuntosCampaign>(
                    new(campaign.Id, beneficiaryId),
                    ct
                );
            }
            await SendAsync(new Response(campaign.Id), 201, ct);
        }
        catch (UniqueConstraintException)
        {
            AddError("Ya existe una campaña con esa comunidad y edición");
            await SendErrorsAsync(cancellation: ct);
        }
    }

    private async Task SendGraphQlErrors(IEnumerable<IClientError> errors, CancellationToken token)
    {
        foreach (var clientError in errors)
            AddError(clientError.Message);

        await SendErrorsAsync(cancellation: token);
    }
}