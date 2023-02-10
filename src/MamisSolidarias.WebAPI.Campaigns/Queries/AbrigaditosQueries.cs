using HotChocolate.AspNetCore.Authorization;
using HotChocolate.Types;
using MamisSolidarias.Infrastructure.Campaigns;
using MamisSolidarias.Infrastructure.Campaigns.Models.Abrigaditos;

namespace MamisSolidarias.WebAPI.Campaigns.Queries;

[ExtendObjectType("Query")]
public class AbrigaditosQueries
{
    [Authorize(Policy = "CanRead")]
    [UseFirstOrDefault]
    [UseProjection]
    public IQueryable<AbrigaditosCampaign> GetAbrigaditosCampaign(CampaignsDbContext db, int id)
    {
        return db.AbrigaditosCampaigns.Where(c => c.Id == id);
    }

    [Authorize(Policy = "CanRead")]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<AbrigaditosCampaign> GetAbrigaditosCampaigns(CampaignsDbContext db)
    {
        return db.AbrigaditosCampaigns;
    }
}