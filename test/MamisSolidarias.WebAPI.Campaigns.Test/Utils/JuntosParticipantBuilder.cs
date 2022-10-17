using Bogus;
using Bogus.DataSets;
using MamisSolidarias.Infrastructure.Campaigns;
using MamisSolidarias.Infrastructure.Campaigns.Models;

namespace MamisSolidarias.WebAPI.Campaigns.Utils;

internal sealed class JuntosParticipantBuilder
{
    private static readonly Faker<JuntosParticipant> Generator = new Faker<JuntosParticipant>()
        .RuleFor(t => t.Id, f => f.IndexGlobal + 1)
        .RuleFor(t => t.Gender, f => f.PickRandom<BeneficiaryGender>())
        .RuleFor(t => t.State, f => f.PickRandom<ParticipantState>())
        .RuleFor(t => t.BeneficiaryId, f => f.IndexGlobal + 1)
        .RuleFor(t => t.DonationType, f => f.PickRandom<DonationType>())
        .RuleFor(t => t.DonorId, f => f.IndexGlobal + 1)
        .RuleFor(t => t.DonorName, f => f.Name.FullName())
        .RuleFor(t => t.ShoeSize, f => $"{f.Random.Number(10, 50)}")
        .RuleFor(t => t.DonationDropOffPoint, f => f.Address.City());

    private readonly JuntosParticipant _JuntosParticipant = Generator.Generate();

    private readonly CampaignsDbContext? _dbContext;

    public JuntosParticipantBuilder(CampaignsDbContext? db = null)
    {
        _dbContext = db;
    }

    public JuntosParticipantBuilder(JuntosParticipant obj)
    {
        _JuntosParticipant = obj;
    }

    public JuntosParticipantBuilder WithCampaign(JuntosCampaign campaign)
    {
        _JuntosParticipant.CampaignId = campaign.Id;
        _JuntosParticipant.Campaign = campaign;
        return this;
    }

    public JuntosParticipantBuilder WithDonationDropOff(string? donationDropOff)
    {
        _JuntosParticipant.DonationDropOffPoint = donationDropOff;
        return this;
    }

    public JuntosParticipantBuilder WithShoeSize(string? shoeSize)
    {
        _JuntosParticipant.ShoeSize = shoeSize;
        return this;
    }

    public JuntosParticipantBuilder WithDonorName(string? donorName)
    {
        _JuntosParticipant.DonorName = donorName;
        return this;
    }

    public JuntosParticipantBuilder WithDonationType(DonationType type)
    {
        _JuntosParticipant.DonationType = type;
        return this;
    }

    public JuntosParticipantBuilder WithBeneficiaryId(int id)
    {
        _JuntosParticipant.BeneficiaryId = id;
        return this;
    }

    public JuntosParticipantBuilder WithId(int id)
    {
        _JuntosParticipant.Id = id;
        return this;
    }

    public JuntosParticipantBuilder WithGender(BeneficiaryGender gender)
    {
        _JuntosParticipant.Gender = gender;
        return this;
    }

    public JuntosParticipantBuilder WithState(ParticipantState state)
    {
        _JuntosParticipant.State = state;
        return this;
    }

    public JuntosParticipant Build()
    {
        _dbContext?.Add(_JuntosParticipant);
        _dbContext?.SaveChanges();
        _dbContext?.ChangeTracker.Clear();
        return _JuntosParticipant;
    }

    public static implicit operator JuntosParticipant(JuntosParticipantBuilder b)
    {
        return b.Build();
    }

    public static implicit operator JuntosParticipantBuilder(JuntosParticipant b)
    {
        return new(b);
    }
}