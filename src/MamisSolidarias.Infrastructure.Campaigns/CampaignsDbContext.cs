using MamisSolidarias.Infrastructure.Campaigns.Models;
using Microsoft.EntityFrameworkCore;

namespace MamisSolidarias.Infrastructure.Campaigns;

public class CampaignsDbContext: DbContext
{
    public DbSet<User> Users { get; set; }
    public CampaignsDbContext(DbContextOptions<CampaignsDbContext> options) : base(options)
    {
    }
    
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(
            model =>
            {
                model.HasKey(t => t.Id);
                model.Property(t => t.Id).ValueGeneratedOnAdd();
                model.Property(t => t.Name)
                    .IsRequired()
                    .HasMaxLength(500);

            }
        );



    }
    
}