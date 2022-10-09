using FastEndpoints;
using MamisSolidarias.Infrastructure.Campaigns;

namespace MamisSolidarias.WebAPI.Campaigns.Endpoints.Campaigns.Mochi.POST;

internal sealed class Endpoint : Endpoint<Request, Response>
{
    private readonly DbAccess _db;

    // public Endpoint(CampaignsDbContext dbContext, DbAccess? dbAccess = null)
    // {
    //     _db = dbAccess ?? new DbAccess(dbContext);
    // }

    public override void Configure()
    {
        Post("/campaigns/mochi");
        Policies(Utils.Security.Policies.CanWrite);
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {


    }


}