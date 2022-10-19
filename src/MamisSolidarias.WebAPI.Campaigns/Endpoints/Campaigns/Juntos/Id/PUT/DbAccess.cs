using MamisSolidarias.Infrastructure.Campaigns;
using MamisSolidarias.Infrastructure.Campaigns.Models;
using Microsoft.EntityFrameworkCore;

namespace MamisSolidarias.WebAPI.Campaigns.Endpoints.Campaigns.Juntos.Id.PUT;

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
    
    public virtual async Task SaveParticipants(IEnumerable<JuntosParticipant> participants, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(_dbContext);
        await _dbContext.JuntosParticipants.AddRangeAsync(participants, ct);
    }

    public virtual async Task SaveChanges(CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(_dbContext);
        await _dbContext.SaveChangesAsync(ct);
    }

    public virtual async Task DeleteParticipants(int campaignId,IEnumerable<int> reqRemovedBeneficiaries, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(_dbContext);
        var participants = await _dbContext.JuntosParticipants
            .AsNoTracking()
            .Where(t => t.CampaignId == campaignId && reqRemovedBeneficiaries.Contains(t.BeneficiaryId))
            .ToListAsync(ct);
        
        _dbContext.JuntosParticipants.RemoveRange(participants);
    }

    public virtual Task<JuntosCampaign?> GetCampaign(int reqId, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(_dbContext);
        return _dbContext.JuntosCampaigns
            .AsTracking()
            .FirstOrDefaultAsync(t=> t.Id == reqId, ct);
    }
}