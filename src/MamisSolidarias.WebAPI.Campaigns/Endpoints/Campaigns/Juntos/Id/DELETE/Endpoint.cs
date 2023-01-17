using FastEndpoints;
using MamisSolidarias.Infrastructure.Campaigns;

namespace MamisSolidarias.WebAPI.Campaigns.Endpoints.Campaigns.Juntos.Id.DELETE;

internal sealed class Endpoint : Endpoint<Request>
{
    private readonly DbAccess _db;

    public Endpoint(CampaignsDbContext dbContext, DbAccess? dbAccess = null)
    {
        _db = dbAccess ?? new DbAccess(dbContext);
    }

    public override void Configure()
    {
        Delete("campaigns/juntos/{id}");
        Policies(Utils.Security.Policies.CanWrite);
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        try
        {
            var campaignExists = await _db.CampaignExists(req.Id, ct);
            if (!campaignExists)
            {
                await SendNotFoundAsync(ct);
                return;
            }

            await _db.DeleteCampaign(req.Id, ct);
            await SendOkAsync(ct);
        }
        catch
        {
            await SendErrorsAsync(500, ct);
        }
    }
}