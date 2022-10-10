using Bogus;
using MamisSolidarias.Infrastructure.Campaigns;
using MamisSolidarias.Infrastructure.Campaigns.Models;

namespace MamisSolidarias.WebAPI.Campaigns.Utils;

internal sealed class MochiParticipantBuilder
{
    private static readonly Faker<MochiParticipant> Generator = new Faker<MochiParticipant>()
        .RuleFor(t=> t.Id, f => f.IndexFaker)
        .RuleFor(t=> t.BeneficiaryGender, f=> f.PickRandom<BeneficiaryGender>())
        .RuleFor(t=> t.BeneficiaryId, f=> f.UniqueIndex)
        .RuleFor(t=> t.BeneficiaryName, f=> f.Name.FullName())
        .RuleFor(t=> t.DonationType,f=> f.PickRandom<MochiDonationType>())
        .RuleFor(t=> t.DonorId, f=> f.UniqueIndex)
        .RuleFor(t=> t.DonorName, f=> f.Name.FullName())
        .RuleFor(t=> t.SchoolCycle, f => f.PickRandom<SchoolCycle>());
    
    
    private readonly MochiParticipant _mochiParticipant = Generator.Generate();

    private readonly CampaignsDbContext? _dbContext;

    public MochiParticipantBuilder(CampaignsDbContext? db) => _dbContext = db;
    public MochiParticipantBuilder() {}
    public MochiParticipantBuilder(MochiParticipant obj) => _mochiParticipant = obj;

    public MochiParticipant Build()
    {
        _dbContext?.Add(_mochiParticipant);
        _dbContext?.SaveChanges();
        _dbContext?.ChangeTracker.Clear();
        return _mochiParticipant;
    }
    
    public MochiParticipantBuilder WithCampaignId(int id)
    {
        _mochiParticipant.CampaignId = id;
        return this;
    }
    
    public MochiParticipantBuilder WithCampaign(Mochi? campaign)
    {
        _mochiParticipant.CampaignId = campaign?.Id ?? 0;
        _mochiParticipant.Campaign = campaign;
        return this;
    }


    public MochiParticipantBuilder WithDonor(string? donorName, int? id, MochiDonationType? donationType)
    {
        _mochiParticipant.DonationType = donationType;
        _mochiParticipant.DonorId = id;
        _mochiParticipant.DonorName = donorName;
        return this;
    }
    
    public MochiParticipantBuilder WithoutDonor()
    {
        _mochiParticipant.DonationType = null;
        _mochiParticipant.DonorId = null;
        _mochiParticipant.DonorName = null;
        return this;
    }

    public MochiParticipantBuilder WithSchoolCycle(SchoolCycle schoolCycle)
    {
        _mochiParticipant.SchoolCycle = schoolCycle;
        return this;
    }
    
    public MochiParticipantBuilder WithDonorName(string donorName)
    {
        _mochiParticipant.DonorName = donorName;
        return this;
    }
    
    public MochiParticipantBuilder WithDonorId(int donorId)
    {
        _mochiParticipant.DonorId = donorId;
        return this;
    }
    public MochiParticipantBuilder WithDonationType(MochiDonationType donationType)
    {
        _mochiParticipant.DonationType = donationType;
        return this;
    }

    public MochiParticipantBuilder WithBeneficiaryName(string beneficiaryName)
    {
        _mochiParticipant.BeneficiaryName = beneficiaryName;
        return this;
    }
    

    public MochiParticipantBuilder WithBeneficiaryId(int beneficiaryId)
    {
        _mochiParticipant.BeneficiaryId = beneficiaryId;
        return this;
    }

    public MochiParticipantBuilder WithId(int id)
    {
        _mochiParticipant.Id = id;
        return this;
    }

    public MochiParticipantBuilder WithBeneficiaryGender(BeneficiaryGender beneficiaryGender)
    {
        _mochiParticipant.BeneficiaryGender = beneficiaryGender;
        return this;
    }
    
    

    public static implicit operator MochiParticipant(MochiParticipantBuilder b) => b.Build();
    public static implicit operator MochiParticipantBuilder(MochiParticipant b) => new(b);

}