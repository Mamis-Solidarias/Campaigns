using FastEndpoints;
using MamisSolidarias.GraphQlClient;
using MamisSolidarias.Infrastructure.Campaigns;
using MamisSolidarias.Infrastructure.Campaigns.Models;
using MamisSolidarias.WebAPI.Campaigns.Endpoints.Campaigns.Mochi.Id.POST;
using MamisSolidarias.WebAPI.Campaigns.Extensions;
using StrawberryShake;

namespace MamisSolidarias.WebAPI.Campaigns.Endpoints.Campaigns.Mochi.Id.PUT;

internal sealed class Endpoint : Endpoint<Request>
{
    private readonly DbAccess _db;
    private readonly IGraphQlClient _graphQlClient;

    public Endpoint(IGraphQlClient graphQlClient,CampaignsDbContext dbContext, DbAccess? dbAccess = null)
    {
        _db = dbAccess ?? new DbAccess(dbContext);
        _graphQlClient = graphQlClient;
    }

    public override void Configure()
    {
        Put("campaigns/mochi/{id}");
        Policies(Utils.Security.Policies.CanWrite);
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var mochi = await _db.GetMochiAsync(req.Id, ct);
        if (mochi is null)
        {
            await SendNotFoundAsync(ct);
            return;
        }
        
        await _db.DeleteParticipantsAsync(req.RemovedBeneficiaries, ct);
        
        var participants = new List<MochiParticipant>();
        
        foreach (var id in req.AddedBeneficiaries)
        {
            var response = await _graphQlClient.GetBeneficiary.ExecuteAsync(id, ct);
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
                BeneficiaryId = id,
                BeneficiaryName =
                    $"{response.Data.Beneficiary.FirstName.ToLower()} {response.Data.Beneficiary.LastName.ToLower()}",
                SchoolCycle = response.Data.Beneficiary.Education?.Cycle.Map(),
                CampaignId = mochi.Id
            };
            participants.Add(entry);
        }
        
        await _db.SaveParticipantsAsync(participants, ct);
        await _db.SaveChangesAsync(ct);
        await SendOkAsync(ct);
    }


}