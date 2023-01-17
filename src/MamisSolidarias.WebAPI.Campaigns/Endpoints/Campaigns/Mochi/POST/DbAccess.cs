using MamisSolidarias.Infrastructure.Campaigns;
using MamisSolidarias.Infrastructure.Campaigns.Models.Mochi;

namespace MamisSolidarias.WebAPI.Campaigns.Endpoints.Campaigns.Mochi.POST;

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

    public virtual async Task AddMochiCampaign(MochiCampaign campaign, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(_dbContext);
        await _dbContext.MochiCampaigns.AddAsync(campaign, ct);
        await _dbContext.SaveChangesAsync(ct);
    }
}