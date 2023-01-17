using Bogus;
using MamisSolidarias.Infrastructure.Campaigns;
using MamisSolidarias.Infrastructure.Campaigns.Models.Mochi;
using MamisSolidarias.WebAPI.Campaigns.Utils.Base;

namespace MamisSolidarias.WebAPI.Campaigns.Utils.Mochi;

internal sealed class MochiParticipantBuilder : ParticipantWithDonorBuilder<MochiParticipant>
{
    public MochiParticipantBuilder(CampaignsDbContext? db = null) : base(db)
    {
    }


    public MochiParticipantBuilder(MochiParticipant obj) : base(obj)
    {
    }

    protected override void SetUpGenerator(Faker<MochiParticipant> generator)
    {
        base.SetUpGenerator(generator);
        generator
            .RuleFor(t => t.SchoolCycle, f => f.PickRandom<SchoolCycle>().OrNull(f));
    }


    public MochiParticipantBuilder WithSchoolCycle(SchoolCycle schoolCycle)
    {
        _participant.SchoolCycle = schoolCycle;
        return this;
    }
}