using HotChocolate.AspNetCore.Authorization;
using HotChocolate.Types;
using MamisSolidarias.Infrastructure.Campaigns;
using MamisSolidarias.Infrastructure.Campaigns.Models;
using MamisSolidarias.Infrastructure.Campaigns.Models.Base;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MamisSolidarias.WebAPI.Campaigns.Queries;

[ExtendObjectType("Query")]
public class MochiQueries
{
    [Authorize(Policy = "CanRead")]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<MochiCampaign> GetMochiEditions([FromServices] CampaignsDbContext dbContext) => 
        dbContext.MochiCampaigns.AsNoTracking();

    [Authorize(Policy = "CanRead")]
    [UseFirstOrDefault]
    [UseProjection]
    public IQueryable<MochiCampaign> GetMochiEditionById([FromServices] CampaignsDbContext dbContext, int id) => 
        dbContext.MochiCampaigns.AsNoTracking().Where(x => x.Id == id);

    [Authorize(Policy = "CanRead")]
    [UseFirstOrDefault]
    [UseProjection]
    public IQueryable<MochiCampaign> GetMochiEdition([FromServices] CampaignsDbContext dbContext, string edition,
        string community) =>
        dbContext.MochiCampaigns.AsNoTracking().Where(x => x.Edition == edition && x.CommunityId == community);


    [Authorize(Policy = "CanRead")]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<MochiParticipant> GetMochiParticipants([FromServices] CampaignsDbContext dbContext,
        int campaignId, MochiParticipantFilters? filter = null)
    {
        var query = dbContext.MochiParticipants
            .AsNoTracking()
            .Where(t => t.CampaignId == campaignId)
            .AsQueryable();

        if (filter is null)
            return query;

        if (filter.BeneficiaryGender is not null)
            query = query.Where(t => t.BeneficiaryGender == filter.BeneficiaryGender);

        if (filter.BeneficiaryName is not null)
            query = query.Where(t => t.BeneficiaryName.ToLower().Contains(filter.BeneficiaryName.ToLower()));

        if (filter.SchoolCycle is not null)
            query = query.Where(t => t.SchoolCycle == filter.SchoolCycle);

        if (filter.KitOrMoney is not null)
            query = query.Where(t => t.DonationType == filter.KitOrMoney);

        return query;
    }
    

    /// <param name="BeneficiaryName">Part of the name of the beneficiary</param>
    /// <param name="BeneficiaryGender">Gender of the beneficiary</param>
    /// <param name="SchoolCycle">School Cycle of the beneficiary</param>
    /// <param name="KitOrMoney">Whether the donor is donating a kit or money</param>
    public sealed record MochiParticipantFilters(
        string? BeneficiaryName,
        BeneficiaryGender? BeneficiaryGender,
        SchoolCycle? SchoolCycle,
        DonationType? KitOrMoney
    );
}