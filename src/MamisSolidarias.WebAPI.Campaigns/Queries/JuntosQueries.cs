using HotChocolate;
using HotChocolate.AspNetCore.Authorization;
using HotChocolate.Types;
using MamisSolidarias.Infrastructure.Campaigns;
using MamisSolidarias.Infrastructure.Campaigns.Models.Base;
using MamisSolidarias.Infrastructure.Campaigns.Models.JuntosALaPar;

namespace MamisSolidarias.WebAPI.Campaigns.Queries;

[ExtendObjectType("Query")]
public class JuntosQueries
{
    [Authorize(Policy = "CanRead")]
    [UseFirstOrDefault]
    [UseProjection]
    public IQueryable<JuntosCampaign> GetJuntosCampaign(CampaignsDbContext context, string community, string edition)
    {
        return context.JuntosCampaigns.Where(t => t.CommunityId == community && t.Edition == edition);
    }

    [Authorize(Policy = "CanRead")]
    [UseFirstOrDefault]
    [UseProjection]
    public IQueryable<JuntosCampaign> GetJuntosCampaignById(CampaignsDbContext context, int id)
    {
        return context.JuntosCampaigns.Where(t => t.Id == id);
    }

    [Authorize(Policy = "CanRead")]
    [UseProjection]
    public IQueryable<JuntosCampaign> GetJuntosCampaigns(CampaignsDbContext context)
    {
        return context.JuntosCampaigns;
    }
}

[ExtendObjectType(typeof(JuntosCampaign))]
public class JuntosCampaignsExtensions
{
    [Authorize(Policy = "CanRead")]
    public List<Shoe> GetShoeDetails([Parent] JuntosCampaign campaign, CampaignsDbContext dbContext)
    {
        return dbContext.JuntosCampaigns
            .Where(t => t.Id == campaign.Id)
            .SelectMany(t => t.Participants)
            .GroupBy(t => new { t.ShoeSize, t.BeneficiaryGender })
            .Select(t => new Shoe(t.Count(), t.Key.BeneficiaryGender, t.Key.ShoeSize))
            .ToList();
    }

    public sealed record Shoe(int Count, BeneficiaryGender Gender, int? Size);
}