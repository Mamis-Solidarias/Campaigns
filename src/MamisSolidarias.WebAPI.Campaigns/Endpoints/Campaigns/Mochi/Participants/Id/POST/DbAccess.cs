using HotChocolate.Language;
using MamisSolidarias.Infrastructure.Campaigns;
using MamisSolidarias.Infrastructure.Campaigns.Models;
using Microsoft.EntityFrameworkCore;

namespace MamisSolidarias.WebAPI.Campaigns.Endpoints.Campaigns.Mochi.Participants.Id.POST;

internal  class DbAccess
{
    private readonly CampaignsDbContext? _dbContext;

    public DbAccess()
    {
    }

    public DbAccess(CampaignsDbContext? dbContext)
    {
        _dbContext = dbContext;
    }

    public virtual Task<MochiParticipant?> GetParticipantAsync(int id, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(_dbContext);
        return _dbContext.MochiParticipants.AsNoTracking().FirstOrDefaultAsync(t=> t.Id == id, ct);
    }

    public virtual async Task SaveParticipantAsync(MochiParticipant participant, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(_dbContext);
        _dbContext.MochiParticipants.Update(participant);
        await _dbContext.SaveChangesAsync(ct);
    }
}