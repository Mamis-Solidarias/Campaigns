using EntityFramework.Exceptions.Common;
using FastEndpoints;
using MamisSolidarias.GraphQlClient;
using MamisSolidarias.Infrastructure.Campaigns;
using MamisSolidarias.Infrastructure.Campaigns.Models;
using MamisSolidarias.WebAPI.Campaigns.Extensions;
using StrawberryShake;

namespace MamisSolidarias.WebAPI.Campaigns.Endpoints.Campaigns.Mochi.POST;

internal sealed class Endpoint : Endpoint<Request, Response>
{
    private readonly DbAccess _db;
    private readonly IGraphQlClient _graphQlClient;

    public Endpoint(CampaignsDbContext dbContext, IGraphQlClient graphQlClient, DbAccess? dbAccess = null)
    {
        _db = dbAccess ?? new DbAccess(dbContext);
        _graphQlClient = graphQlClient;
    }

    public override void Configure()
    {
        Post("/campaigns/mochi");
        Policies(Utils.Security.Policies.CanWrite);
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var campaign = new MochiCampaign
        {
            CommunityId = req.CommunityId.Trim(),
            Edition = req.Edition.Trim(),
            Description = req.Description,
            Provider = req.Provider
        };

        foreach (var beneficiaryId in req.Beneficiaries)
        {
            var response = await _graphQlClient.GetBeneficiaryWithEducation.ExecuteAsync(beneficiaryId, ct);

            var hasErrors = await response.HandleErrors(
                async t => await SendForbiddenAsync(t),
                async (errors, t) => await SendGraphQlErrors(errors, t),
                ct
            );

            if (hasErrors)
                return;

            if (response.Data?.Beneficiary is null)
            {
                AddError("Beneficiario no valido");
                await SendErrorsAsync(409, ct);
                return;
            }

            var entry = new MochiParticipant
            {
                BeneficiaryGender = response.Data.Beneficiary.Gender.Map(),
                BeneficiaryId = beneficiaryId,
                BeneficiaryName =
                    $"{response.Data.Beneficiary.FirstName.ToLower()} {response.Data.Beneficiary.LastName.ToLower()}",
                SchoolCycle = response.Data.Beneficiary.Education?.Cycle.Map()
            };

            campaign.Participants.Add(entry);
        }

        try
        {
            await _db.AddMochiCampaign(campaign, ct);
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