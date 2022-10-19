using MamisSolidarias.Infrastructure.Campaigns.Models;
using Microsoft.EntityFrameworkCore;

namespace MamisSolidarias.Infrastructure.Campaigns;
#pragma warning disable CS8618
public class CampaignsDbContext: DbContext
{
    public DbSet<MochiCampaign> MochiCampaigns { get; set; }
    public DbSet<MochiParticipant> MochiParticipants { get; set; }
    
    public DbSet<JuntosCampaign> JuntosCampaigns { get; set; }
    public DbSet<JuntosParticipant> JuntosParticipants { get; set; }


    public CampaignsDbContext(DbContextOptions<CampaignsDbContext> options) : base(options)

    { }
    
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        new MochiConfigurator().Configure(modelBuilder.Entity<MochiCampaign>());
        new MochiParticipantConfiguration().Configure(modelBuilder.Entity<MochiParticipant>());
        new JuntosCampaignConfigurator().Configure(modelBuilder.Entity<JuntosCampaign>());
        new JuntosParticipantConfigurator().Configure(modelBuilder.Entity<JuntosParticipant>());
    }
}

#pragma warning restore CS8618