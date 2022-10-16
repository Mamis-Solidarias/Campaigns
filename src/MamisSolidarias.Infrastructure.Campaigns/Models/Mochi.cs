using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MamisSolidarias.Infrastructure.Campaigns.Models;

public class Mochi
{
    public int Id { get; set; }
    public string Edition { get; set; } = string.Empty;
    public string CommunityId { get; set; } = string.Empty;
    public string? Provider { get; set; }
    public string? Description { get; set; }
    public virtual ICollection<MochiParticipant> Participants { get; set; } = new List<MochiParticipant>();
    
}

internal sealed class MochiConfigurator : IEntityTypeConfiguration<Mochi>
{
    public void Configure(EntityTypeBuilder<Mochi> builder)
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

        builder.HasMany(t => t.Participants)
            .WithOne(t => t.Campaign)
            .HasPrincipalKey(t => t.Id)
            .HasForeignKey(t => t.CampaignId);
    }
}