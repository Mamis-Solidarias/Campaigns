using MamisSolidarias.Infrastructure.Campaigns;

namespace MamisSolidarias.WebAPI.Campaigns.Endpoints.Test;

internal class DbService
{
    private readonly CampaignsDbContext? _dbContext;

    public DbService() { }
    public DbService(CampaignsDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
}