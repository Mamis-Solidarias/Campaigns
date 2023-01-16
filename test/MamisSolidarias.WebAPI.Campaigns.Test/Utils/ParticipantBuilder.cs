using Bogus;
using MamisSolidarias.Infrastructure.Campaigns;
using MamisSolidarias.Infrastructure.Campaigns.Models.Base;

namespace MamisSolidarias.WebAPI.Campaigns.Utils;

internal abstract class ParticipantBuilder<T> where T : Participant
{
    protected readonly CampaignsDbContext? _db;
    private readonly Faker<T> _generator = new();
    protected readonly T _participant;

    protected ParticipantBuilder(CampaignsDbContext? db = null)
    {
        _db = db;
        // ReSharper disable once VirtualMemberCallInConstructor
        SetUpGenerator(_generator);
        _participant = _generator.Generate();
    }

    protected ParticipantBuilder(T participant)
    {
        _participant = participant;
        // ReSharper disable once VirtualMemberCallInConstructor
        SetUpGenerator(_generator);
    }

    public ParticipantBuilder<T> WithCampaignId(int campaignId)
    {
        _participant.CampaignId = campaignId;
        return this;
    }

    public ParticipantBuilder<T> WithId(int id)
    {
        _participant.Id = id;
        return this;
    }

    public ParticipantBuilder<T> WithBeneficiaryId(int beneficiaryId)
    {
        _participant.BeneficiaryId = beneficiaryId;
        return this;
    }

    public ParticipantBuilder<T> WithGender(BeneficiaryGender gender)
    {
        _participant.BeneficiaryGender = gender;
        return this;
    }

    protected virtual void SetUpGenerator(Faker<T> generator)
    {
        generator
            .RuleFor(t => t.Id, f => f.IndexGlobal + 1)
            .RuleFor(t => t.BeneficiaryId, f => f.IndexGlobal + 1)
            .RuleFor(t => t.BeneficiaryGender, f => f.PickRandom<BeneficiaryGender>())
            .RuleFor(t => t.BeneficiaryName, f => f.Name.FullName())
            ;
    }

    public T Build()
    {
        _db?.Add(_participant);
        _db?.SaveChanges();
        _db?.ChangeTracker.Clear();
        return _participant;
    }

    public static implicit operator Participant(ParticipantBuilder<T> b)
    {
        return b.Build();
    }
}