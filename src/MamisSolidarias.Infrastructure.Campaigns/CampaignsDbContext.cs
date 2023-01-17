using MamisSolidarias.Infrastructure.Campaigns.Models;
using MamisSolidarias.Infrastructure.Campaigns.Models.Abrigaditos;
using MamisSolidarias.Infrastructure.Campaigns.Models.Mochi;
using Microsoft.EntityFrameworkCore;

namespace MamisSolidarias.Infrastructure.Campaigns;
#pragma warning disable CS8618
public class CampaignsDbContext : DbContext
{
    public CampaignsDbContext(DbContextOptions<CampaignsDbContext> options) : base(options)
    {
    }

    public DbSet<MochiCampaign> MochiCampaigns => Set<MochiCampaign>();
    public DbSet<MochiParticipant> MochiParticipants => Set<MochiParticipant>();

    public DbSet<JuntosCampaign> JuntosCampaigns { get; set; }
    public DbSet<JuntosParticipant> JuntosParticipants { get; set; }

    public DbSet<AbrigaditosCampaign> AbrigaditosCampaigns => Set<AbrigaditosCampaign>();
    public DbSet<AbrigaditosParticipant> AbrigaditosParticipants => Set<AbrigaditosParticipant>();


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        new MochiConfigurator().Configure(modelBuilder.Entity<MochiCampaign>());
        new MochiParticipantConfiguration().Configure(modelBuilder.Entity<MochiParticipant>());
        new JuntosCampaignConfigurator().Configure(modelBuilder.Entity<JuntosCampaign>());
        new JuntosParticipantConfigurator().Configure(modelBuilder.Entity<JuntosParticipant>());
        new AbrigaditosCampaignConfigurator().Configure(modelBuilder.Entity<AbrigaditosCampaign>());
        new AbrigaditosParticipantConfigurator().Configure(modelBuilder.Entity<AbrigaditosParticipant>());
    }
}

#pragma warning restore CS8618