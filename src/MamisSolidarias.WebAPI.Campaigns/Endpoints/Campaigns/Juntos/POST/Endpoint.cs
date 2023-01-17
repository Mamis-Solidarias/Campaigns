using EntityFramework.Exceptions.Common;
using FastEndpoints;
using MamisSolidarias.GraphQlClient;
using MamisSolidarias.Infrastructure.Campaigns;
using MamisSolidarias.Infrastructure.Campaigns.Models.JuntosALaPar;
using MamisSolidarias.Messages;
using MamisSolidarias.WebAPI.Campaigns.Extensions;
using MassTransit;
using StrawberryShake;

namespace MamisSolidarias.WebAPI.Campaigns.Endpoints.Campaigns.Juntos.POST;

internal sealed class Endpoint : Endpoint<Request, Response>
{
    private readonly IBus _bus;
    private readonly CampaignsDbContext _db;
    private readonly IGraphQlClient _graphQlClient;

    public Endpoint(CampaignsDbContext dbContext, IBus bus, IGraphQlClient graphQlClient)
    {
        _graphQlClient = graphQlClient;
        _bus = bus;
        _db = dbContext;
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

        var communityExecutor = await _graphQlClient
            .GetCommunity
            .ExecuteAsync(campaign.CommunityId, ct);

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
            await _db.JuntosCampaigns.AddAsync(campaign, ct);
            await _db.SaveChangesAsync(ct);

            await _bus.PublishBatch(
                req.Beneficiaries.Distinct().Select(beneficiaryId =>
                    new ParticipantAddedToJuntosCampaign(campaign.Id, beneficiaryId)
                ),
                ct
            );
            
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