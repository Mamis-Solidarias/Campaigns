using System.Collections.Generic;
using System.Linq;
using Bogus;
using MamisSolidarias.Infrastructure.Campaigns;
using MamisSolidarias.Infrastructure.Campaigns.Models;

namespace MamisSolidarias.WebAPI.Campaigns.Utils;

internal sealed class MochiBuilder
{
    private static readonly Faker<Mochi> Generator = new Faker<Mochi>()
        .RuleFor(t => t.Id, f => f.IndexGlobal + 1)
        .RuleFor(t => t.Edition, f => f.Date.Recent().Year.ToString())
        .RuleFor(t=> t.CommunityId, f=> f.Name.FirstName()[..2].ToUpper())
        .RuleFor(t=> t.Participants, 
            f=> Enumerable.Range(0,f.Random.Int(0,10))
                .Select(_=> new MochiParticipantBuilder().Build())
                .ToList()
        );
    
    private readonly Mochi _mochi = Generator.Generate();

    private readonly CampaignsDbContext? _dbContext;

    public MochiBuilder(CampaignsDbContext? db) => _dbContext = db;
    public MochiBuilder(Mochi obj) => _mochi = obj;
    
    public MochiBuilder WithId(int id)
    {
        _mochi.Id = id;
        return this;
    }
    public MochiBuilder WithEdition(string edition)
    {
        _mochi.Edition = edition;
        return this;
    }
    
    public MochiBuilder WithCommunityId(string communityId)
    {
        _mochi.CommunityId = communityId;
        return this;
    }
    
    public MochiBuilder WithParticipants(IEnumerable<MochiParticipant> participants)
    {
        _mochi.Participants = participants.ToList();
        return this;
    }

    public Mochi Build()
    {
        _dbContext?.Add(_mochi);
        _dbContext?.SaveChanges();
        _dbContext?.ChangeTracker.Clear();
        return _mochi;
    }

    public static implicit operator Mochi(MochiBuilder b) => b.Build();
    public static implicit operator MochiBuilder(Mochi b) => new(b);

}