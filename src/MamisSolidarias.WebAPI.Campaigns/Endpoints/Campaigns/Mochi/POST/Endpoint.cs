using EntityFramework.Exceptions.Common;
using FastEndpoints;
using MamisSolidarias.Infrastructure.Campaigns;
using MamisSolidarias.Infrastructure.Campaigns.Models.Mochi;
using MamisSolidarias.Messages;
using MassTransit;

namespace MamisSolidarias.WebAPI.Campaigns.Endpoints.Campaigns.Mochi.POST;

internal sealed class Endpoint : Endpoint<Request, Response>
{
    private readonly IBus _bus;
    private readonly CampaignsDbContext _db;


    public Endpoint(CampaignsDbContext dbContext, IBus bus)
    {
        _db = dbContext;
        _bus = bus;
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
        try
        {
            await _db.MochiCampaigns.AddAsync(campaign, ct);
            await _db.SaveChangesAsync(ct);
            await _bus.PublishBatch(
                req.Beneficiaries.Select(
                    t => new ParticipantAddedToMochiCampaign(t, campaign.Id)
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
}