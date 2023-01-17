using MamisSolidarias.Infrastructure.Campaigns;
using MamisSolidarias.Infrastructure.Campaigns.Models.Mochi;
using Microsoft.EntityFrameworkCore;

namespace MamisSolidarias.WebAPI.Campaigns.Endpoints.Campaigns.Mochi.Id.POST;

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

    public virtual Task<MochiCampaign?> GetCampaignAsync(int id, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(_dbContext);
        return _dbContext.MochiCampaigns
            .AsNoTracking()
            .Include(t=> t.Participants)
            .FirstOrDefaultAsync(t=> t.Id == id, ct);
    }

    public virtual async Task SaveCampaignAsync(MochiCampaign campaign, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(_dbContext);
        await _dbContext.MochiCampaigns.AddAsync(campaign, ct);
        await _dbContext.SaveChangesAsync(ct);
    }
}