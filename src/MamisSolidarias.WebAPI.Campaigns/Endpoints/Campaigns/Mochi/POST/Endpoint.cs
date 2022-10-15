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
        var campaign = new Infrastructure.Campaigns.Models.Mochi
        {
            CommunityId = req.CommunityId.Trim(),
            Edition = req.Edition.Trim(),
        };
        
        foreach (var beneficiaryId in req.Beneficiaries)
        {
            var response = await _graphQlClient.GetBeneficiary.ExecuteAsync(beneficiaryId, ct);
            if (response.IsErrorResult())
            {
                if (response.Errors.Any(t => t.Code is "AUTH_NOT_AUTHORIZED"))
                    await SendForbiddenAsync(ct);
                else
                {
                    foreach (var error in response.Errors)
                        AddError(error.Message);
                    
                    await SendErrorsAsync(cancellation: ct);
                }
                return;
            }
            
            if (response.Data?.Beneficiary is null)
            {
                AddError("Beneficiario no valido");
                await SendErrorsAsync(409,cancellation: ct);
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
            await SendAsync(new Response(campaign.Edition, campaign.CommunityId), 201, ct);
        }
        catch (UniqueConstraintException)
        {
            AddError("Ya existe una campaña con esa comunidad y edición");
            await SendErrorsAsync(cancellation: ct);
        }

    }

    
    
}