using MamisSolidarias.Infrastructure.Campaigns;
using Microsoft.EntityFrameworkCore;

namespace MamisSolidarias.WebAPI.Campaigns.Endpoints.Campaigns.Juntos.Id.DELETE;

internal class DbAccess
{
    private readonly CampaignsDbContext? _dbContext;

    public DbAccess(){}
    public DbAccess(CampaignsDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public virtual Task<bool> CampaignExists(int campaignId, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(_dbContext);
        return _dbContext.JuntosCampaigns.AnyAsync(t=> t.Id == campaignId, ct);
    }

    public virtual Task DeleteCampaign(int campaignId, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(_dbContext);
        return _dbContext.JuntosCampaigns
            .Where(t=> t.Id == campaignId)
            .ExecuteDeleteAsync(ct);
    }
}