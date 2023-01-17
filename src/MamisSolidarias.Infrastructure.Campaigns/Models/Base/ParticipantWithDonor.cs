using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace MamisSolidarias.Infrastructure.Campaigns.Models.Base;

public abstract class ParticipantWithDonor : Participant
{
    /// <summary>
    /// State of the participant with respect to the donation
    /// </summary>
    public ParticipantState State { get; set; } = ParticipantState.MissingDonor;

    /// <summary>
    /// Id of the donor related to the participant
    /// </summary>
    public int? DonorId { get; set; }
    
    /// <summary>
    /// Type of the donation
    /// </summary>
    public DonationType? DonationType { get; set; }
    
    /// <summary>
    /// Where the donation will be made
    /// </summary>
    public string? DonationDropOffPoint { get; set; }
    
    /// <summary>
    /// Name of the donor
    /// </summary>
    public string? DonorName { get; set; }

    /// <summary>
    /// Id of the donation
    /// </summary>
    public Guid? DonationId { get; set; }
}

internal abstract class ParticipantWithDonorConfigurator<TParticipant> : ParticipantConfigurator<TParticipant>
    where TParticipant : ParticipantWithDonor
{
    public new void Configure(EntityTypeBuilder<TParticipant> builder)
    {
        builder.Property(t => t.DonorId)
            .IsRequired(false);
        builder.Property(t => t.DonorName)
            .IsRequired(false);
        builder.Property(t => t.DonationType)
            .IsRequired(false);
        builder.Property(t => t.DonationDropOffPoint)
            .IsRequired(false);
        builder.Property(t => t.State)
            .IsRequired()
            .HasConversion(new EnumToStringConverter<ParticipantState>())
            .HasDefaultValue(ParticipantState.MissingDonor);
        
        builder.HasIndex(t => new {t.BeneficiaryId, t.DonorId, t.CampaignId})
            .IsUnique();
        
        builder.Property(t=> t.DonationId)
            .IsRequired(false);

        builder.HasIndex(t => t.DonationId)
            .IsUnique();
        
        base.Configure(builder);
    }

}