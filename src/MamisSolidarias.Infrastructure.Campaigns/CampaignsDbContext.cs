using MamisSolidarias.Infrastructure.Campaigns.Models;
using Microsoft.EntityFrameworkCore;

namespace MamisSolidarias.Infrastructure.Campaigns;

public class CampaignsDbContext: DbContext
{
    public DbSet<Mochi> MochiCampaigns { get; set; }
    public DbSet<MochiParticipant> MochiParticipants { get; set; }

    public CampaignsDbContext(DbContextOptions<CampaignsDbContext> options) : base(options)
    { }
    
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        new MochiConfigurator().Configure(modelBuilder.Entity<Mochi>());
        new MochiParticipantConfiguration().Configure(modelBuilder.Entity<MochiParticipant>());
    }
    
}