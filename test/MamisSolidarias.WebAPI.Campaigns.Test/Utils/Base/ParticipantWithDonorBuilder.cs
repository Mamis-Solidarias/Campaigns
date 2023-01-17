using Bogus;
using MamisSolidarias.Infrastructure.Campaigns;
using MamisSolidarias.Infrastructure.Campaigns.Models.Base;

namespace MamisSolidarias.WebAPI.Campaigns.Utils.Base;

internal class ParticipantWithDonorBuilder<TParticipant> : ParticipantBuilder<TParticipant> 
    where TParticipant : ParticipantWithDonor
{
    protected ParticipantWithDonorBuilder(CampaignsDbContext? db = null): base(db)
    { }

    protected ParticipantWithDonorBuilder(TParticipant participant) : base(participant)
    { }

    protected override void SetUpGenerator(Faker<TParticipant> generator)
    {
        base.SetUpGenerator(generator);
        generator
            .RuleFor(t => t.State, f => f.PickRandom<ParticipantState>())
            .RuleFor(t => t.DonorId, (f, p) => p.State is ParticipantState.MissingDonor ? null : f.IndexGlobal + 1)
            .RuleFor(t => t.DonorName, (f, p) => p.DonorId is null ? null : f.Name.FullName())
            .RuleFor(t => t.DonationType, (f, p) => p.DonorId is null ? null : f.PickRandom<DonationType>())
            .RuleFor(t => t.DonationDropOffPoint, (f, p) => p.DonorId is null ? null : f.Address.FullAddress())
            ;
    }
    
    public ParticipantWithDonorBuilder<TParticipant> WithState(ParticipantState state)
    {
        _participant.State = state;
        return this;
    }
    
    public ParticipantWithDonorBuilder<TParticipant> WithDonorId(int donorId)
    {
        _participant.DonorId = donorId;
        _participant.DonorName = new Faker().Name.FullName();
        _participant.DonationDropOffPoint = new Faker().Address.FullAddress();
        return this;
    }

    public ParticipantWithDonorBuilder<TParticipant> AsNoDonorAssigned()
    {
        _participant.State = ParticipantState.MissingDonor;
        _participant.DonorId = null;
        _participant.DonorName = null;
        _participant.DonationType = null;
        _participant.DonationDropOffPoint = null;
        return this;
    }

    public ParticipantWithDonorBuilder<TParticipant> AsNoDonationAssigned()
    {
        
        _participant.State = ParticipantState.MissingDonation;
        _participant.DonationDropOffPoint = new Faker().Address.FullAddress();
        _participant.DonorId = new Faker().UniqueIndex + 1;
        _participant.DonationType = DonationType.Money;
        _participant.DonorName = new Faker().Name.FullName();
        return this;

    }

    public ParticipantWithDonorBuilder<TParticipant> AsDonationAssigned()
    {
        _participant.State = ParticipantState.DonationReceived;
        _participant.DonationDropOffPoint = new Faker().Address.FullAddress();
        _participant.DonorId = new Faker().UniqueIndex + 1;
        _participant.DonationType = DonationType.Money;
        _participant.DonorName = new Faker().Name.FullName();
        return this;
    }
}