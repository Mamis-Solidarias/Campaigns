using EntityFramework.Exceptions.Common;
using FastEndpoints;
using MamisSolidarias.GraphQlClient;
using MamisSolidarias.Infrastructure.Campaigns;
using MamisSolidarias.Infrastructure.Campaigns.Models;
using MamisSolidarias.WebAPI.Campaigns.Extensions;
using StrawberryShake;

namespace MamisSolidarias.WebAPI.Campaigns.Endpoints.Campaigns.Mochi.Id.POST;

internal sealed class Endpoint : Endpoint<Request, Response>
{
    private readonly DbAccess _db;
    private readonly IGraphQlClient _graphQl;

    public Endpoint(CampaignsDbContext dbContext,IGraphQlClient graphQlClient, DbAccess? dbAccess = null)
    {
        _db = dbAccess ?? new DbAccess(dbContext);
        _graphQl = graphQlClient;
    }

    public override void Configure()
    {
        Post("campaigns/mochi/{PreviousCampaignId}");
        Policies(Utils.Security.Policies.All);
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        Infrastructure.Campaigns.Models.Mochi? previousEdition = await _db.GetCampaignAsync(req.PreviousCampaignId, ct);
        if (previousEdition is null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        var newEdition = new Infrastructure.Campaigns.Models.Mochi
        {
            CommunityId = req.CommunityId,
            Edition = req.Edition
        };

        foreach (var participant in previousEdition.Participants)
        {
            var response = await _graphQl.GetBeneficiary.ExecuteAsync(participant.BeneficiaryId, ct);
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
                BeneficiaryId = participant.Id,
                BeneficiaryName =
                    $"{response.Data.Beneficiary.FirstName.ToLower()} {response.Data.Beneficiary.LastName.ToLower()}",
                SchoolCycle = response.Data.Beneficiary.Education?.Cycle.Map()
            };
            newEdition.Participants.Add(entry);
        }
        
        try
        {
            await _db.SaveCampaignAsync(newEdition, ct);
            await SendAsync(new Response(newEdition.Edition, newEdition.CommunityId),201, ct);
        }
        catch (UniqueConstraintException)
        {
            AddError("Ya existe una campaña con la misma comunidad y edición");
            await SendErrorsAsync(cancellation: ct);
        }
    }


}