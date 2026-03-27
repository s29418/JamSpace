using JamSpace.Application.Common.Interfaces;
using JamSpace.Domain.Entities;
using JamSpace.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace JamSpace.Infrastructure.Repositories;

public class TeamRepository : ITeamRepository
{
    private readonly JamSpaceDbContext _db;

    public TeamRepository(JamSpaceDbContext db) => _db = db;

    public async Task<Team?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await _db.Teams
            .Include(t => t.CreatedBy)
            .Include(t => t.Members).ThenInclude(m => m.User)
            .FirstOrDefaultAsync(t => t.Id == id, ct);
    }

    public async Task<List<Team>> GetByUserIdAsync(Guid userId, CancellationToken ct)
    {
        return await _db.Teams
            .Where(t => t.Members.Any(m => m.UserId == userId))
            .Include(t => t.CreatedBy)
            .Include(t => t.Members)
            .ThenInclude(m => m.User)
            .ToListAsync(ct);
    }
    

    public async Task AddAsync(Team team, CancellationToken ct) => await _db.Teams.AddAsync(team, ct);

    public void Remove(Team team) => _db.Teams.Remove(team);
}