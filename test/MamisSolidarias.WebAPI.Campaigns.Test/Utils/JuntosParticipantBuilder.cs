using Bogus;
using MamisSolidarias.Infrastructure.Campaigns;
using MamisSolidarias.Infrastructure.Campaigns.Models;
using MamisSolidarias.Infrastructure.Campaigns.Models.Base;

namespace MamisSolidarias.WebAPI.Campaigns.Utils;

internal sealed class JuntosParticipantBuilder
{
    private static readonly Faker<JuntosParticipant> Generator = new Faker<JuntosParticipant>()
        .RuleFor(t => t.Id, f => f.IndexGlobal + 1)
        .RuleFor(t => t.Gender, f => f.PickRandom<BeneficiaryGender>())
        .RuleFor(t => t.State, f => f.PickRandom<ParticipantState>())
        .RuleFor(t => t.BeneficiaryId, f => f.IndexGlobal + 1)
        .RuleFor(t => t.DonorId, f => f.IndexGlobal + 1)
        .RuleFor(t => t.DonorName, f => f.Name.FullName())
        .RuleFor(t => t.ShoeSize, f => f.Random.Number(10, 50));

    private readonly CampaignsDbContext? _dbContext;

    private readonly JuntosParticipant _juntosParticipant = Generator.Generate();

    public JuntosParticipantBuilder(CampaignsDbContext? db = null)
    {
        _dbContext = db;
    }

    public JuntosParticipantBuilder(JuntosParticipant obj)
    {
        _juntosParticipant = obj;
    }

    public JuntosParticipantBuilder WithCampaignId(int campaignId)
    {
        _juntosParticipant.CampaignId = campaignId;
        return this;
    }

    public JuntosParticipantBuilder WithCampaign(JuntosCampaign campaign)
    {
        _juntosParticipant.CampaignId = campaign.Id;
        _juntosParticipant.Campaign = campaign;
        return this;
    }

    public JuntosParticipantBuilder WithDonationDropOff(string? donationDropOff)
    {
        _juntosParticipant.DonationDropOffPoint = donationDropOff;
        return this;
    }

    public JuntosParticipantBuilder WithShoeSize(int? shoeSize)
    {
        _juntosParticipant.ShoeSize = shoeSize;
        return this;
    }

    public JuntosParticipantBuilder WithDonorName(string? donorName)
    {
        _juntosParticipant.DonorName = donorName;
        return this;
    }

    public JuntosParticipantBuilder WithDonationType(DonationType type)
    {
        _juntosParticipant.DonationType = type;
        return this;
    }

    public JuntosParticipantBuilder WithBeneficiaryId(int id)
    {
        _juntosParticipant.BeneficiaryId = id;
        return this;
    }

    public JuntosParticipantBuilder WithId(int id)
    {
        _juntosParticipant.Id = id;
        return this;
    }

    public JuntosParticipantBuilder WithGender(BeneficiaryGender gender)
    {
        _juntosParticipant.Gender = gender;
        return this;
    }

    public JuntosParticipantBuilder WithState(ParticipantState state)
    {
        _juntosParticipant.State = state;
        return this;
    }

    public JuntosParticipant Build()
    {
        _dbContext?.Add(_juntosParticipant);
        _dbContext?.SaveChanges();
        _dbContext?.ChangeTracker.Clear();
        return _juntosParticipant;
    }

    public static implicit operator JuntosParticipant(JuntosParticipantBuilder b)
    {
        return b.Build();
    }

    public static implicit operator JuntosParticipantBuilder(JuntosParticipant b)
    {
        return new JuntosParticipantBuilder(b);
    }
}