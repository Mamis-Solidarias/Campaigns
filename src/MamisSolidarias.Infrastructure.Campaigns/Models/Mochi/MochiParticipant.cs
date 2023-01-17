using MamisSolidarias.Infrastructure.Campaigns.Models.Base;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MamisSolidarias.Infrastructure.Campaigns.Models.Mochi;

public enum SchoolCycle
{
    PreSchool,
    PrimarySchool,
    MiddleSchool,
    HighSchool
}

public class MochiParticipant : ParticipantWithDonor
{
    public SchoolCycle? SchoolCycle { get; set; }
}

internal sealed class MochiParticipantConfiguration : ParticipantWithDonorConfigurator<MochiParticipant>
{
    public new void Configure(EntityTypeBuilder<MochiParticipant> builder)
    {
        base.Configure(builder);
        builder.Property(t => t.SchoolCycle).IsRequired(false);
    }
}