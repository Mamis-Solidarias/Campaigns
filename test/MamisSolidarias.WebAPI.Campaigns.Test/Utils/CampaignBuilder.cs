using System.Collections.Generic;
using System.Linq;
using Bogus;
using MamisSolidarias.Infrastructure.Campaigns;
using MamisSolidarias.Infrastructure.Campaigns.Models.Base;

namespace MamisSolidarias.WebAPI.Campaigns.Utils;

internal abstract class CampaignBuilder<TCampaign, TParticipant> 
    where TCampaign: Campaign<TParticipant>
    where TParticipant : Participant
{
    private readonly Faker<TCampaign> _generator = new();
    protected readonly CampaignsDbContext? _db;
    protected readonly TCampaign _campaign;
    protected virtual void SetUpGenerator(Faker<TCampaign> generator)
    {
        generator
            .RuleFor(t => t.Id, f => f.IndexGlobal + 1)
            .RuleFor(t => t.Edition, f => f.Date.Recent().Year.ToString())
            .RuleFor(t => t.CommunityId, f => f.Name.FirstName()[..2].ToUpper())
            .RuleFor(t => t.Description, f => f.Lorem.Sentence())
            .RuleFor(t => t.Provider, f => f.Company.CompanyName())
            ;
    }

    protected CampaignBuilder(CampaignsDbContext? db)
    {
        _db = db;
        // ReSharper disable once VirtualMemberCallInConstructor
        SetUpGenerator(_generator);
        _campaign = _generator.Generate();
    }

    protected CampaignBuilder(TCampaign obj)
    {
        _campaign = obj;
    }

    public TCampaign Build()
    {
        _db?.Add(_campaign);
        _db?.SaveChanges();
        _db?.ChangeTracker.Clear();
        return _campaign;
    }
    
    public CampaignBuilder<TCampaign,TParticipant> WithId(int id)
    {
        _campaign.Id = id;
        return this;
    }
    
    public CampaignBuilder<TCampaign,TParticipant> WithEdition(string edition)
    {
        _campaign.Edition = edition;
        return this;
    }

    public CampaignBuilder<TCampaign,TParticipant> WithCommunityId(string id)
    {
        _campaign.CommunityId = id;
        return this;
    }
    
    public CampaignBuilder<TCampaign,TParticipant> WithDescription(string description)
    {
        _campaign.Description = description;
        return this;
    }
    
    public CampaignBuilder<TCampaign,TParticipant> WithProvider(string provider)
    {
        _campaign.Provider = provider;
        return this;
    }

    public CampaignBuilder<TCampaign,TParticipant> WithParticipants(List<TParticipant> participants)
    {
        participants.ForEach(t=> t.CampaignId = _campaign.Id);
        _campaign.Participants = participants;
        return this;
    }
    
    public CampaignBuilder<TCampaign,TParticipant> WithParticipants(IEnumerable<ParticipantBuilder<TParticipant>> participants)
    {
        _campaign.Participants = participants
            .Select(t=> t.WithCampaignId(_campaign.Id).Build())
            .ToList();
        return this;
    }

    public static implicit operator TCampaign (CampaignBuilder<TCampaign,TParticipant> b)
    {
        return b.Build();
    }
    
}