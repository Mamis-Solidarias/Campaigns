using MamisSolidarias.Infrastructure.Campaigns.Models.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MamisSolidarias.Infrastructure.Campaigns.Models;

public class JuntosCampaign
{
    /// <summary>
    /// 
    /// </summary>
    public int Id { get; set; }
    /// <summary>
    /// Description of the campaign edition
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Provider for the campaign edition
    /// </summary>
    public string? Provider { get; set; }

    /// <summary>
    /// Edition of the campaign
    /// </summary>
    public string Edition { get; set; } = string.Empty;

    /// <summary>
    /// Community assigned to the campaign edition
    /// </summary>
    public string CommunityId { get; set; } = string.Empty;
    
    /// <summary>
    /// Expected amount of money collected for the campaign edition
    /// </summary>
    public decimal FundraiserGoal { get; set; }
    
    /// <summary>
    /// Participants of the campaign edition
    /// </summary>
    public virtual ICollection<JuntosParticipant> Participants { get; set; } = new List<JuntosParticipant>();

    /// <summary>
    /// List of Donation's IDs for the campaign edition
    /// </summary>
    public virtual ICollection<CampaignDonation> Donations { get; set; } = new List<CampaignDonation>();
}

internal sealed class JuntosCampaignConfigurator : IEntityTypeConfiguration<JuntosCampaign>
{
    public void Configure(EntityTypeBuilder<JuntosCampaign> builder)
    {
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id).ValueGeneratedOnAdd().IsRequired();
        builder.Property(t => t.Edition)
            .IsRequired()
            .HasMaxLength(10);
        
        builder.HasIndex(t=> new {t.Edition, t.CommunityId}).IsUnique();

        builder.Property(t => t.CommunityId)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(t => t.Description)
            .HasMaxLength(500)
            .IsRequired(false);
        
        builder.Property(t=> t.Provider)
            .HasMaxLength(300)
            .IsRequired(false);

        builder.Property(t => t.FundraiserGoal)
            .IsRequired();

        builder.HasMany(t => t.Participants)
            .WithOne(t => t.Campaign)
            .HasPrincipalKey(t => t.Id)
            .HasForeignKey(t => t.CampaignId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.OwnsMany(t => t.Donations,
            r =>
            {
                r.ToTable("JuntosDonations");
                r.WithOwner().HasForeignKey("CampaignId");
                r.HasKey(x => x.Id);
                r.Property(x => x.Id).ValueGeneratedNever();
            });

    }
}