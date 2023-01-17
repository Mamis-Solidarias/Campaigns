using MamisSolidarias.Infrastructure.Campaigns;
using MamisSolidarias.Infrastructure.Campaigns.Models.Mochi;
using Microsoft.EntityFrameworkCore;

namespace MamisSolidarias.WebAPI.Campaigns.Endpoints.Campaigns.Mochi.Id.DELETE;

internal class DbAccess
{
    private readonly CampaignsDbContext? _dbContext;

    public DbAccess()
    {
    }

    public DbAccess(CampaignsDbContext? dbContext)
    {
        _dbContext = dbContext;
    }

    public virtual Task<MochiCampaign?> GetMochiAsync(int id, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(_dbContext);
        return _dbContext.MochiCampaigns.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id, ct);
    }

    public virtual async Task DeleteMochiAsync(MochiCampaign mochi, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(_dbContext);
        _dbContext.MochiCampaigns.Remove(mochi);
        await _dbContext.SaveChangesAsync(ct);
    }
}