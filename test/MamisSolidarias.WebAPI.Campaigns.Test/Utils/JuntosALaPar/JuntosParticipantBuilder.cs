using Bogus;
using MamisSolidarias.Infrastructure.Campaigns;
using MamisSolidarias.Infrastructure.Campaigns.Models.JuntosALaPar;
using MamisSolidarias.WebAPI.Campaigns.Utils.Base;

namespace MamisSolidarias.WebAPI.Campaigns.Utils.JuntosALaPar;

internal sealed class JuntosParticipantBuilder : ParticipantBuilder<JuntosParticipant>
{
    public JuntosParticipantBuilder(CampaignsDbContext? db = null)
    :base(db) { }
    
    public JuntosParticipantBuilder(JuntosParticipant obj)
        :base(obj) { }

    protected override void SetUpGenerator(Faker<JuntosParticipant> generator)
    {
        base.SetUpGenerator(generator);
        generator
            .RuleFor(t => t.ShoeSize, f => f.Random.Number(10, 50));
    }

    public JuntosParticipantBuilder WithShoeSize(int? shoeSize)
    {
        _participant.ShoeSize = shoeSize;
        return this;
    }
}