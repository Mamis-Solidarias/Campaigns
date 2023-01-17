using EntityFramework.Exceptions.Common;
using FastEndpoints;
using MamisSolidarias.Infrastructure.Campaigns;
using MamisSolidarias.Infrastructure.Campaigns.Models.Base;
using MamisSolidarias.Infrastructure.Campaigns.Models.Mochi;
using Microsoft.EntityFrameworkCore;

namespace MamisSolidarias.WebAPI.Campaigns.Endpoints.Campaigns.Mochi.Id.POST;

internal sealed class Endpoint : Endpoint<Request, Response>
{
    private readonly CampaignsDbContext _db;

    public Endpoint(CampaignsDbContext dbContext)
    {
        _db = dbContext;
    }

    public override void Configure()
    {
        Post("campaigns/mochi/{PreviousCampaignId}");
        Policies(Utils.Security.Policies.All);
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        try
        {
            var previousEdition = await _db.MochiCampaigns
                .AsTracking()
                .Include(t => t.Participants)
                .SingleAsync(t => t.Id == req.PreviousCampaignId, ct);

            var newEdition = new MochiCampaign
            {
                CommunityId = req.CommunityId,
                Edition = req.Edition,
                Description = previousEdition.Description,
                Provider = previousEdition.Provider,
                Participants = previousEdition.Participants
                    .Select(t => new MochiParticipant
                    {
                        BeneficiaryId = t.BeneficiaryId,
                        BeneficiaryName = t.BeneficiaryName,
                        BeneficiaryGender = t.BeneficiaryGender,
                        State = ParticipantState.MissingDonor,
                        SchoolCycle = t.SchoolCycle
                    }).ToList()
            };

            await _db.MochiCampaigns.AddAsync(newEdition, ct);
            await _db.SaveChangesAsync(ct);

            await SendAsync(new Response(newEdition.Id), 201, ct);
        }
        catch (InvalidOperationException)
        {
            await SendNotFoundAsync(ct);
        }
        catch (UniqueConstraintException)
        {
            AddError("Ya existe una campaña con la misma comunidad y edición");
            await SendErrorsAsync(cancellation: ct);
        }
    }
}