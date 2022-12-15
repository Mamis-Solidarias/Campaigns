using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MamisSolidarias.Infrastructure.Campaigns.Models;


public class JuntosParticipant
{
    public int Id { get; set; }
    
    public BeneficiaryGender Gender { get; set; }
    
    public int? ShoeSize { get; set; }

    public ParticipantState State { get; set; } = ParticipantState.MissingDonor;
    public int CampaignId { get; set; }
    public virtual JuntosCampaign Campaign { get; set; } = null!;
    
    public int BeneficiaryId { get; set; }
    public int? DonorId { get; set; }
    public string? DonorName { get; set; }
    public DonationType? DonationType { get; set; }
    public string? DonationDropOffPoint { get; set; }
}

internal sealed class JuntosParticipantConfigurator : IEntityTypeConfiguration<JuntosParticipant>
{
    public void Configure(EntityTypeBuilder<JuntosParticipant> builder)
    {
        builder.Property(t => t.DonorId)
            .IsRequired(false);

        builder.Property(t => t.DonorName)
            .IsRequired(false)
            .HasMaxLength(200);

        builder.Property(t => t.DonationType)
            .IsRequired(false);
        
        builder.Property(t=> t.DonationDropOffPoint)
            .IsRequired(false)
            .HasMaxLength(200);
        
        builder.Property(t => t.Id)
            .ValueGeneratedOnAdd()
            .IsRequired();
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Gender)
            .IsRequired();

        builder.Property(t => t.ShoeSize)
            .IsRequired(false)
            .HasMaxLength(20);

        builder.Property(t => t.State)
            .IsRequired();

        builder.Property(t => t.CampaignId)
            .IsRequired();

        builder.Property(t => t.BeneficiaryId)
            .IsRequired();

        builder.HasOne(t => t.Campaign)
            .WithMany(t => t.Participants)
            .HasForeignKey(t => t.CampaignId)
            .HasPrincipalKey(t => t.Id);
    }
}