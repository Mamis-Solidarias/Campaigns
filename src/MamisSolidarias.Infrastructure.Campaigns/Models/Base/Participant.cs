using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace MamisSolidarias.Infrastructure.Campaigns.Models.Base;

public abstract class Participant
{
    /// <summary>
    ///     Id of the participant
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    ///     Id of the campaign related to the participant
    /// </summary>
    public int CampaignId { get; set; }

    /// <summary>
    ///     Id of the beneficiary related to the participant
    /// </summary>
    public int BeneficiaryId { get; set; }

    /// <summary>
    ///     Name of the beneficiary
    /// </summary>
    public string BeneficiaryName { get; set; } = string.Empty;

    /// <summary>
    ///     Gender of the beneficiary
    /// </summary>
    public BeneficiaryGender BeneficiaryGender { get; set; }
}

internal abstract class ParticipantConfigurator<TParticipant> : IEntityTypeConfiguration<TParticipant>
    where TParticipant : Participant
{
    public void Configure(EntityTypeBuilder<TParticipant> builder)
    {
        builder.Property(t => t.BeneficiaryName)
            .IsRequired();
        builder.Property(t => t.BeneficiaryGender)
            .IsRequired()
            .HasConversion(new EnumToStringConverter<BeneficiaryGender>());

        builder.Property(t => t.Id)
            .ValueGeneratedOnAdd()
            .IsRequired();
        builder.HasKey(t => t.Id);
        builder.Property(t => t.CampaignId)
            .IsRequired();
        builder.Property(t => t.BeneficiaryId)
            .IsRequired();
        builder.HasIndex(t => t.BeneficiaryName);
    }
}