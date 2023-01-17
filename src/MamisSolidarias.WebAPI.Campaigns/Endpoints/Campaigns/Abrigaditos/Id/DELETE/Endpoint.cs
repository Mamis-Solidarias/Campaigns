using FastEndpoints;
using MamisSolidarias.Infrastructure.Campaigns;
using Microsoft.EntityFrameworkCore;

namespace MamisSolidarias.WebAPI.Campaigns.Endpoints.Campaigns.Abrigaditos.Id.Delete;

internal sealed class Endpoint : Endpoint<Request>
{
    private readonly CampaignsDbContext _db;

    public Endpoint(CampaignsDbContext dbContext)
    {
        _db = dbContext;
    }

    public override void Configure()
    {
        Delete("campaigns/abrigaditos/{id}");
        Policies(Utils.Security.Policies.CanWrite);
        Description(t=> t.WithTags("Abrigaditos"));
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var deletedCampaigns = await _db.AbrigaditosCampaigns
            .Where(t => t.Id == req.Id)
            .ExecuteDeleteAsync(ct);

        if (deletedCampaigns is 0)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        await SendOkAsync(ct);
    }
}