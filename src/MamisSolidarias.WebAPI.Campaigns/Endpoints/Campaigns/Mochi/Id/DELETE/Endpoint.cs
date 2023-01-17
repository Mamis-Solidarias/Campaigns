using FastEndpoints;
using MamisSolidarias.Infrastructure.Campaigns;
using Microsoft.EntityFrameworkCore;

namespace MamisSolidarias.WebAPI.Campaigns.Endpoints.Campaigns.Mochi.Id.DELETE;

internal sealed class Endpoint : Endpoint<Request>
{
    private readonly CampaignsDbContext _db;

    public Endpoint(CampaignsDbContext dbContext)
    {
        _db = dbContext;
    }

    public override void Configure()
    {
        Delete("/campaigns/mochi/{id}");
        Policies(Utils.Security.Policies.CanWrite);
        Description(t=> t.WithTags("Una Mochi como la Tuya"));

    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var deletedRows = await _db.MochiCampaigns
            .Where(x => x.Id == req.Id)
            .ExecuteDeleteAsync(ct);

        if (deletedRows is 0)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        await SendOkAsync(ct);
    }
}