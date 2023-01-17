using FastEndpoints;
using MamisSolidarias.Infrastructure.Campaigns;

namespace MamisSolidarias.WebAPI.Campaigns.Endpoints.Campaigns.Mochi.Id.DELETE;

internal sealed class Endpoint : Endpoint<Request>
{
    private readonly DbAccess _db;

    public Endpoint(CampaignsDbContext dbContext, DbAccess? dbAccess = null)
    {
        _db = dbAccess ?? new DbAccess(dbContext);
    }

    public override void Configure()
    {
        Delete("/campaigns/mochi/{id}");
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

        await _db.DeleteMochiAsync(mochi, ct);
        await SendOkAsync(ct);
    }
}