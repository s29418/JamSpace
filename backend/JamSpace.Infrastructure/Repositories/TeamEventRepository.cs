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
    
    public async Task<TeamEvent?> GetById(Guid eventId, CancellationToken ct)
    {
        return await _db.TeamEvents
            .FirstOrDefaultAsync(e => e.Id == eventId, ct);
    }

    public async Task<IReadOnlyList<TeamEvent>> GetTeamEvents(Guid teamId, DateTimeOffset from, DateTimeOffset to, 
        CancellationToken ct)
    {
        return await _db.TeamEvents
            .Where(e => e.TeamId == teamId &&
                        e.StartDateTime >= from &&
                        e.StartDateTime <= to)
            .OrderBy(e => e.StartDateTime)
            .ToListAsync(ct);
    }

    public async Task<bool> WasEventCreatedByUser(Guid eventId, Guid userId, CancellationToken ct)
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