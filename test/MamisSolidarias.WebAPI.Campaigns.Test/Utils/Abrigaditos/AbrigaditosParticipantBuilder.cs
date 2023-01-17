using Bogus;
using MamisSolidarias.Infrastructure.Campaigns;
using MamisSolidarias.Infrastructure.Campaigns.Models.Abrigaditos;
using MamisSolidarias.WebAPI.Campaigns.Utils.Base;

namespace MamisSolidarias.WebAPI.Campaigns.Utils.Abrigaditos;

internal sealed class AbrigaditosParticipantBuilder 
: ParticipantBuilder<AbrigaditosParticipant>
{
    public AbrigaditosParticipantBuilder(CampaignsDbContext? db = null)
    : base(db){ }

    public AbrigaditosParticipantBuilder(AbrigaditosParticipant participant) : base(participant){}
    
    protected override void SetUpGenerator(Faker<AbrigaditosParticipant> generator)
    {
        base.SetUpGenerator(generator);
        generator
            .RuleFor(t => t.ShirtSize, faker => faker.PickRandom("L", "M", "S", "XL", "XXL", null))
            ;
    }
    
    public AbrigaditosParticipantBuilder WithShirtSize(string? shirtSize)
    {
        _participant.ShirtSize = shirtSize;
        return this;
    }
}