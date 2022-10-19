using FastEndpoints;
using MamisSolidarias.GraphQlClient;
using MamisSolidarias.Infrastructure.Campaigns;
using MamisSolidarias.Infrastructure.Campaigns.Models;
using MamisSolidarias.WebAPI.Campaigns.Extensions;
using StrawberryShake;

namespace MamisSolidarias.WebAPI.Campaigns.Endpoints.Campaigns.Juntos.Id.PUT;

internal sealed class Endpoint : Endpoint<Request>
{
    private readonly DbAccess _db;
    private readonly IGraphQlClient _graphQlClient;

    public Endpoint(IGraphQlClient graphQlClient, CampaignsDbContext dbContext, DbAccess? dbAccess = null)
    {
        _graphQlClient = graphQlClient;
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
            await _db.DeleteParticipants(req.Id,req.RemovedBeneficiaries, ct);
        
        var participants = new List<JuntosParticipant>();
        
        foreach (var id in req.AddedBeneficiaries)
        {
            var response = await _graphQlClient.GetBeneficiaryWithClothes.ExecuteAsync(id, ct);

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
                await SendErrorsAsync(409,cancellation: ct);
                return;
            }

            var entry = new JuntosParticipant()
            {
                BeneficiaryId = id,
                CampaignId = campaign.Id,
                Gender = response.Data.Beneficiary.Gender.Map(),
                ShoeSize = response.Data.Beneficiary.Clothes?.ShoeSize
            };
            participants.Add(entry);
        }

        campaign.Description = string.IsNullOrWhiteSpace(req.Description) ? null : req.Description.Trim();
        campaign.Provider = string.IsNullOrWhiteSpace(req.Provider) ? null : req.Provider.Trim();
        campaign.FundraiserGoal = req.FundraiserGoal;
        
        await _db.SaveParticipants(participants, ct);
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
