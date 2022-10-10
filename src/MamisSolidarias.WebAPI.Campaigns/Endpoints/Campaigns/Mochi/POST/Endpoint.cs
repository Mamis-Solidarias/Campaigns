using EntityFramework.Exceptions.Common;
using FastEndpoints;
using MamisSolidarias.GraphQlClient;
using MamisSolidarias.Infrastructure.Campaigns;
using MamisSolidarias.Infrastructure.Campaigns.Models;
using StrawberryShake;
using BeneficiaryGender = MamisSolidarias.Infrastructure.Campaigns.Models.BeneficiaryGender;
using SchoolCycle = MamisSolidarias.Infrastructure.Campaigns.Models.SchoolCycle;

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
                BeneficiaryGender = Map(response.Data.Beneficiary.Gender),
                BeneficiaryId = beneficiaryId,
                BeneficiaryName =
                    $"{response.Data.Beneficiary.FirstName.ToLower()} {response.Data.Beneficiary.LastName.ToLower()}",
                SchoolCycle = Map(response.Data.Beneficiary.Education?.Cycle)
            };
            
            campaign.Participants.Add(entry);
        }

        try
        {
            await _db.AddMochiCampaign(campaign, ct);
            await SendAsync(new Response(campaign.Edition, campaign.CommunityId), 201, ct);
        }
        catch (UniqueConstraintException e)
        {
            AddError("Ya existe una campaña con esa comunidad y edición");
            await SendErrorsAsync(cancellation: ct);
        }

    }

    private static SchoolCycle? Map(GraphQlClient.SchoolCycle? beneficiaryGender)
        => beneficiaryGender switch
        {
            GraphQlClient.SchoolCycle.PreSchool => SchoolCycle.PreSchool,
            GraphQlClient.SchoolCycle.PrimarySchool => SchoolCycle.PrimarySchool,
            GraphQlClient.SchoolCycle.MiddleSchool => SchoolCycle.MiddleSchool,
            GraphQlClient.SchoolCycle.HighSchool => SchoolCycle.HighSchool,
            null => null,
            _ => throw new ArgumentOutOfRangeException(nameof(beneficiaryGender), beneficiaryGender, null)
        };


    private static BeneficiaryGender Map(GraphQlClient.BeneficiaryGender beneficiaryGender)
        => beneficiaryGender switch
        {
            GraphQlClient.BeneficiaryGender.Male => BeneficiaryGender.Male,
            GraphQlClient.BeneficiaryGender.Female => BeneficiaryGender.Female,
            GraphQlClient.BeneficiaryGender.Other => BeneficiaryGender.Other,
            _ => throw new ArgumentOutOfRangeException(nameof(beneficiaryGender), beneficiaryGender, null)
        };
    
}