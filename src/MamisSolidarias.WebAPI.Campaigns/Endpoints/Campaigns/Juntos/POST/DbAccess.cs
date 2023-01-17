using MamisSolidarias.Infrastructure.Campaigns;
using MamisSolidarias.Infrastructure.Campaigns.Models.JuntosALaPar;

namespace MamisSolidarias.WebAPI.Campaigns.Endpoints.Campaigns.Juntos.POST;

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

    public virtual async Task AddCampaign(JuntosCampaign campaign, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(_dbContext);
        await _dbContext.JuntosCampaigns.AddAsync(campaign, ct);
        await _dbContext.SaveChangesAsync(ct);
    }
}