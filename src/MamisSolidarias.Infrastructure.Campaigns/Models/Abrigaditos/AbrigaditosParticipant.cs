using MamisSolidarias.Infrastructure.Campaigns.Models.Base;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MamisSolidarias.Infrastructure.Campaigns.Models.Abrigaditos;

public class AbrigaditosParticipant : Participant
{
    public string? ShirtSize { get; set; }
}

internal sealed class AbrigaditosParticipantConfigurator : ParticipantConfigurator<AbrigaditosParticipant>
{
    public new void Configure(EntityTypeBuilder<AbrigaditosParticipant> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.ShirtSize).IsRequired(false);
    }
}