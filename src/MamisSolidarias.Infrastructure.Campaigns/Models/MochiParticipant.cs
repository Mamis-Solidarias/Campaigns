using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MamisSolidarias.Infrastructure.Campaigns.Models;

public enum MochiDonationType
{
    Bono,
    Kit
}

public enum Schoolcycle
{
    PreSchool,
    PrimarySchool,
    MiddleSchool,
    HighSchool
}

public enum BeneficiaryGender
{
    Male, Female, Other
}

public class MochiParticipant
{
    public int Id { get; set; }
    public int CampaignId { get; set; }
    public virtual Mochi Campaign { get; set; }

    public int BeneficiaryId { get; set; }
    public int? DonorId { get; set; }

    public string BeneficiaryName { get; set; } = string.Empty;
    public string DonorName { get; set; } = string.Empty;
    public BeneficiaryGender BeneficiaryGender { get; set; }
    public Schoolcycle? SchoolCycle { get; set; } 
    public MochiDonationType DonationType { get; set; }
}

internal sealed class MochiParticipantConfiguration: IEntityTypeConfiguration<MochiParticipant>{
    public void Configure(EntityTypeBuilder<MochiParticipant> builder)
    {
        builder.Property(t => t.Id).IsRequired()
            .ValueGeneratedOnAdd();
        builder.HasKey(t => t.Id);
        builder.Property(t => t.CampaignId).IsRequired();
        builder.HasOne(t => t.Campaign)
            .WithMany(t => t.Participants)
            .HasPrincipalKey(t => t.Id)
            .HasForeignKey(t => t.CampaignId)
            .IsRequired();
        builder.Property(t => t.BeneficiaryId)
            .IsRequired();
        builder.Property(t=> t.DonorId)
            .IsRequired(false);

        builder.HasIndex(t => new {t.BeneficiaryId, t.DonorId, t.CampaignId})
            .IsUnique();
        
        builder.Property(t=> t.BeneficiaryName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(t => t.DonorName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(t => t.BeneficiaryGender).IsRequired();

        builder.Property(t => t.SchoolCycle).IsRequired(false);
        builder.Property(t => t.DonationType).IsRequired();
    }
}