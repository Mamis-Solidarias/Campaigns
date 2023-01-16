using System;
using System.Collections.Generic;
using System.Linq;
using Bogus;
using MamisSolidarias.Infrastructure.Campaigns;
using MamisSolidarias.Infrastructure.Campaigns.Models.Base;

namespace MamisSolidarias.WebAPI.Campaigns.Utils;

internal abstract class CampaignWithDonationsBuilder<TCampaign,TParticipant> 
    : CampaignBuilder<TCampaign,TParticipant>
    where TCampaign : CampaignWithDonations<TParticipant>
    where TParticipant : Participant
{
    protected CampaignWithDonationsBuilder(CampaignsDbContext? db) : base(db)
    {
    }

    protected CampaignWithDonationsBuilder(TCampaign obj) : base(obj)
    {
    }

    protected override void SetUpGenerator(Faker<TCampaign> generator)
    {
        base.SetUpGenerator(generator);
        generator
            .RuleFor(t => t.FundraiserGoal, f => f.Random.Decimal(0, 10000))
            .RuleFor(t=> t.Donations, f => Enumerable.Range(0, f.Random.Int(0, 10))
                .Select(_ =>  (CampaignDonation) f.Random.Guid())
                .ToList()
            )
            ;
    }

    public CampaignWithDonationsBuilder<TCampaign, TParticipant> WithFundraiserGoal(decimal value)
    {
        _campaign.FundraiserGoal = value;
        return this;
    }
    
    public CampaignWithDonationsBuilder<TCampaign, TParticipant> WithDonations(IEnumerable<CampaignDonation> donations)
    {
        _campaign.Donations = donations.ToList();
        return this;
    }
    
    public CampaignWithDonationsBuilder<TCampaign, TParticipant> WithDonations(IEnumerable<Guid> donations)
    {
        // ReSharper disable once SuspiciousTypeConversion.Global
        _campaign.Donations = donations.Select(t=> (CampaignDonation)t).ToList();
        return this;
    }
    public CampaignWithDonationsBuilder<TCampaign, TParticipant> WithDonations(params Guid[] donations)
    {
        return WithDonations(donations.AsEnumerable());
    }
    
    public CampaignWithDonationsBuilder<TCampaign, TParticipant> WithoutDonations()
    {
        _campaign.Donations = new List<CampaignDonation>();
        return this;
    }
    
    public CampaignWithDonationsBuilder<TCampaign, TParticipant> WithDonation(Guid donation)
    {
        _campaign.Donations = new List<CampaignDonation> { donation };
        return this;
    }
    
    public CampaignWithDonationsBuilder<TCampaign, TParticipant> WithDonation(CampaignDonation donation)
    {
        _campaign.Donations = new List<CampaignDonation> { donation };
        return this;
    }
    
}