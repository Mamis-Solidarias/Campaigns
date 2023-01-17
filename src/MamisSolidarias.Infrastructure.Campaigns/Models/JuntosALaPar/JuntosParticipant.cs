using MamisSolidarias.Infrastructure.Campaigns.Models.Base;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MamisSolidarias.Infrastructure.Campaigns.Models.JuntosALaPar;

public class JuntosParticipant : Participant
{
    public int? ShoeSize { get; set; }
}

internal sealed class JuntosParticipantConfigurator : ParticipantConfigurator<JuntosParticipant>
{
    public new void Configure(EntityTypeBuilder<JuntosParticipant> builder)
    {
        base.Configure(builder);
        builder.Property(t => t.ShoeSize)
            .IsRequired(false);
    }
}