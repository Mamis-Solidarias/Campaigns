using HotChocolate.AspNetCore.Authorization;
using MamisSolidarias.Infrastructure.Campaigns;
using MamisSolidarias.Infrastructure.Campaigns.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MamisSolidarias.WebAPI.Campaigns.Queries;

public class MochiQueries
{
    [Authorize(Policy = "CanRead")]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Mochi> GetMochiEditions([FromServices] CampaignsDbContext dbContext)
    {
        return dbContext.MochiCampaigns.AsNoTracking();
    }

    [Authorize(Policy = "CanRead")]
    [UseFirstOrDefault]
    [UseProjection]
    public IQueryable<Mochi> GetSpecificMochiEdition([FromServices] CampaignsDbContext dbContext, int id)
    {
        return dbContext.MochiCampaigns.AsNoTracking().Where(x => x.Id == id);
    }

    [Authorize(Policy = "CanRead")]
    [UseFirstOrDefault]
    [UseProjection]
    public IQueryable<Mochi> GetMochiEdition([FromServices] CampaignsDbContext dbContext, string edition,
        string community)
    {
        return dbContext.MochiCampaigns.AsNoTracking().Where(x => x.Edition == edition && x.CommunityId == community);
    }

    [Authorize(Policy = "CanRead")]
    [UseProjection]
    public IQueryable<MochiParticipant> GetMochiParticipants([FromServices] CampaignsDbContext dbContext, int id)
    {
        return dbContext.MochiParticipants.AsNoTracking().Where(x => x.CampaignId == id);
    }

    [Authorize(Policy = "CanRead")]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<MochiParticipant> GetFilteredMochiParticipants([FromServices] CampaignsDbContext dbContext,
        int campaignId, MochiParticipantFilters filter)
    {
        var query = dbContext.MochiParticipants
            .AsNoTracking()
            .Where(t => t.CampaignId == campaignId)
            .AsQueryable();

        if (filter.BeneficiaryGender is not null)
            query = query.Where(t => t.BeneficiaryGender == filter.BeneficiaryGender);

        if (filter.BeneficiaryName is not null)
            query = query.Where(t => t.BeneficiaryName.ToLower().Contains(filter.BeneficiaryName.ToLower()));

        if (filter.SchoolCycle is not null)
            query = query.Where(t => t.SchoolCycle == filter.SchoolCycle);

        if (filter.KitOrBonus is not null)
            query = query.Where(t => t.DonationType == filter.KitOrBonus);

        return query;
    }
    
    public sealed record MochiParticipantFilters(
        string? BeneficiaryName,
        BeneficiaryGender? BeneficiaryGender,
        SchoolCycle? SchoolCycle,
        MochiDonationType? KitOrBonus
    );
}