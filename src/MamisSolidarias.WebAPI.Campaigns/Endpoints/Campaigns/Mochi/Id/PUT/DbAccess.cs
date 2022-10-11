using MamisSolidarias.Infrastructure.Campaigns;
using MamisSolidarias.Infrastructure.Campaigns.Models;
using Microsoft.EntityFrameworkCore;

namespace MamisSolidarias.WebAPI.Campaigns.Endpoints.Campaigns.Mochi.Id.PUT;

internal class DbAccess
{
    private readonly CampaignsDbContext? _dbContext;

    public DbAccess()
    {
    }

    public DbAccess(CampaignsDbContext? dbContext)
    {
        _dbContext = dbContext;
    }
    
    public virtual Task<Infrastructure.Campaigns.Models.Mochi?> GetMochiAsync(int id, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(_dbContext);
        return _dbContext.MochiCampaigns.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id, ct);
    }

    public virtual async Task DeleteParticipantsAsync(IEnumerable<int> reqRemovedBeneficiaries, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(_dbContext);

        var participants = await _dbContext.MochiParticipants
            .Where(t => reqRemovedBeneficiaries.Contains(t.BeneficiaryId))
            .ToListAsync(ct);
        
        _dbContext.MochiParticipants.RemoveRange(participants);
    }

    public virtual async Task SaveChangesAsync(CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(_dbContext);
        await _dbContext.SaveChangesAsync(ct);
    }

    public virtual Task SaveParticipantsAsync(List<MochiParticipant> participants, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(_dbContext);
        return _dbContext.MochiParticipants.AddRangeAsync(participants, ct);
    }
}