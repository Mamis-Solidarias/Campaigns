using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MamisSolidarias.Infrastructure.Campaigns.Models.Base;

public abstract class Campaign<TParticipant>
    where TParticipant : Participant
{
    /// <summary>
    ///     Id of the campaign
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    ///     Description of the campaign edition
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    ///     Provider for the campaign edition
    /// </summary>
    public string? Provider { get; set; }

    /// <summary>
    ///     Edition of the campaign
    /// </summary>
    public string Edition { get; set; } = string.Empty;

    /// <summary>
    ///     Community assigned to the campaign edition
    /// </summary>
    public string CommunityId { get; set; } = string.Empty;

    public virtual ICollection<TParticipant> Participants { get; set; } = new List<TParticipant>();
}

internal abstract class CampaignConfigurator<TCampaign, TParticipant> : IEntityTypeConfiguration<TCampaign>
    where TCampaign : Campaign<TParticipant>
    where TParticipant : Participant
{
    public void Configure(EntityTypeBuilder<TCampaign> builder)
    {
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id).ValueGeneratedOnAdd().IsRequired();
        builder.Property(t => t.Edition)
            .IsRequired();

        builder.HasIndex(t => new { t.Edition, t.CommunityId }).IsUnique();

        builder.Property(t => t.CommunityId)
            .IsRequired();

        builder.Property(t => t.Description)
            .IsRequired(false);

        builder.Property(t => t.Provider)
            .IsRequired(false);

        builder.HasMany(t => t.Participants)
            .WithOne()
            .HasPrincipalKey(t => t.Id)
            .HasForeignKey(t => t.CampaignId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}