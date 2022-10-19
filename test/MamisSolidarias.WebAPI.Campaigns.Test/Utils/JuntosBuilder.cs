using System.Collections.Generic;
using System.Linq;
using Bogus;
using MamisSolidarias.Infrastructure.Campaigns;
using MamisSolidarias.Infrastructure.Campaigns.Models;

namespace MamisSolidarias.WebAPI.Campaigns.Utils;

internal sealed class JuntosBuilder
{
    private static readonly Faker<JuntosCampaign> Generator = new Faker<JuntosCampaign>()
        .RuleFor(t => t.Id, f => f.IndexGlobal + 1)
        .RuleFor(t => t.Edition, f => f.Date.Recent().Year.ToString())
        .RuleFor(t => t.CommunityId, f => f.Name.FirstName()[..2].ToUpper())
        .RuleFor(t => t.Participants,
            (f, c) => Enumerable.Range(0, f.Random.Int(0, 10))
                .Select(_ =>
                    new JuntosParticipantBuilder()
                        .WithCampaign(c)
                        .Build()
                )
                .ToList()
        )
        .RuleFor(t => t.Description, f => f.Lorem.Sentence())
        .RuleFor(t => t.Provider, f => f.Company.CompanyName())
        .RuleFor(t => t.FundraiserGoal, f => f.Random.Decimal(0, 999999999));

    private readonly CampaignsDbContext? _dbContext;

    private readonly JuntosCampaign _juntos = Generator.Generate();

    public JuntosBuilder(CampaignsDbContext? db)
    {
        _dbContext = db;
    }

    public JuntosBuilder(JuntosCampaign obj)
    {
        _juntos = obj;
    }

    public JuntosBuilder WithId(int id)
    {
        _juntos.Id = id;
        return this;
    }

    public JuntosBuilder WithEdition(string edition)
    {
        _juntos.Edition = edition;
        return this;
    }

    public JuntosBuilder WithCommunityId(string communityId)
    {
        _juntos.CommunityId = communityId;
        return this;
    }

    public JuntosBuilder WithParticipants(IEnumerable<JuntosParticipant> participants)
    {
        _juntos.Participants = participants.ToList();
        return this;
    }

    public JuntosBuilder WithFundraiserGoal(decimal fundraiserGoal)
    {
        _juntos.FundraiserGoal = fundraiserGoal;
        return this;
    }

    public JuntosBuilder WithDescription(string description)
    {
        _juntos.Description = description;
        return this;
    }

    public JuntosBuilder WithProvider(string provider)
    {
        _juntos.Provider = provider;
        return this;
    }

    public JuntosCampaign Build()
    {
        _dbContext?.Add(_juntos);
        _dbContext?.SaveChanges();
        _dbContext?.ChangeTracker.Clear();
        return _juntos;
    }

    public static implicit operator JuntosCampaign(JuntosBuilder b)
    {
        return b.Build();
    }

    public static implicit operator JuntosBuilder(JuntosCampaign b)
    {
        return new JuntosBuilder(b);
    }
}