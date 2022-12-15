using HotChocolate.AspNetCore.Authorization;
using MamisSolidarias.Infrastructure.Campaigns;
using MamisSolidarias.Infrastructure.Campaigns.Models;

namespace MamisSolidarias.WebAPI.Campaigns.Queries;

[ExtendObjectType("Query")]
public class JuntosQueries
{
    [Authorize(Policy = "CanRead")]
    public List<Shoe> GetShoesFromCampaign(int campaignId, CampaignsDbContext dbContext)
    {
        return dbContext.JuntosCampaigns
            .Where(t => t.Id == campaignId)
            .SelectMany(t => t.Participants)
            .GroupBy(t => new { t.ShoeSize, t.Gender })
            .Select(t => new Shoe(t.Count(), t.Key.Gender, t.Key.ShoeSize ?? -1))
            .ToList();

    }
    
    public sealed record Shoe(int Count, BeneficiaryGender Gender, int Size);
}