using JamSpace.Application.Common.Interfaces;
using JamSpace.Domain.Entities;
using JamSpace.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace JamSpace.Infrastructure.Repositories;

public class TeamEventRepository : ITeamEventRepository
{
    private readonly JamSpaceDbContext _db;

    public TeamEventRepository(JamSpaceDbContext db)
    {
        _db = db;
    }
    
    public async Task<TeamEvent?> GetByIdAsync(Guid eventId, CancellationToken ct)
    {
        return await _db.TeamEvents
            .Include(e => e.CreatedBy)
            .FirstOrDefaultAsync(e => e.Id == eventId, ct);
    }

    public async Task<IReadOnlyList<TeamEvent>> GetTeamEventsAsync(Guid teamId, DateTimeOffset from, DateTimeOffset to, 
        CancellationToken ct)
    {
        var earliestPossibleStart = from.AddHours(-24);
        
        var candidates = await _db.TeamEvents
            .Include(e => e.CreatedBy)
            .Where(e => e.TeamId == teamId &&
                        e.StartDateTime < to &&
                        e.StartDateTime > earliestPossibleStart)
            .OrderBy(e => e.StartDateTime)
            .ToListAsync(ct);

        return candidates
            .Where(e => e.StartDateTime.AddMinutes(e.DurationMinutes) > from)
            .ToList();
    }

    public async Task<bool> WasEventCreatedByUserAsync(Guid eventId, Guid userId, CancellationToken ct)
    {
        return await _db.TeamEvents
            .AnyAsync(e => e.Id == eventId && e.CreatedById == userId, ct);
    }

    public async Task AddAsync(TeamEvent teamEvent, CancellationToken ct)
    {
        await _db.TeamEvents.AddAsync(teamEvent, ct);
    }

    public void Remove(TeamEvent teamEvent)
    {
        _db.TeamEvents.Remove(teamEvent);
    }
}
