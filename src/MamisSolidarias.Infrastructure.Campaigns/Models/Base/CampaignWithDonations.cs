using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MamisSolidarias.Infrastructure.Campaigns.Models.Base;

public class CampaignWithDonations<TParticipant> : Campaign<TParticipant> 
    where TParticipant : Participant
{
    /// <summary>
    /// List of Donation's IDs for the campaign edition
    /// </summary>
    public virtual ICollection<CampaignDonation> Donations { get; set; } = new List<CampaignDonation>();
    
    /// <summary>
    /// Monetary goal of the campaign
    /// </summary>
    public decimal FundraiserGoal { get; set; }
}

internal abstract class
    CampaignWithDonationsConfigurator<TCampaign, TParticipant> : CampaignConfigurator<TCampaign, TParticipant>
    where TCampaign : CampaignWithDonations<TParticipant>
    where TParticipant : Participant
{
    private readonly string DonationTableName;

    protected CampaignWithDonationsConfigurator(string DonationTableName)
    {
        this.DonationTableName = DonationTableName;
    }
    
    public new void Configure(EntityTypeBuilder<TCampaign> builder)
    {
        base.Configure(builder);

        builder.OwnsMany(t => t.Donations,
            r =>
            {
                r.ToTable(DonationTableName);
                r.WithOwner().HasForeignKey("CampaignId");
                r.HasKey(x => x.Id);
                r.Property(x => x.Id).ValueGeneratedNever();
            });

        builder.Property(t => t.FundraiserGoal)
            .IsRequired();
    }
}