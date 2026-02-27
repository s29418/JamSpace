using JamSpace.Application.Common.Interfaces;
using JamSpace.Domain.Entities;
using JamSpace.Domain.Enums;
using JamSpace.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace JamSpace.Infrastructure.Repositories;

public class TeamMemberRepository : ITeamMemberRepository
{
    private readonly JamSpaceDbContext _db;

    public TeamMemberRepository(JamSpaceDbContext db) => _db = db;

    public Task<bool> HasRequiredRoleAsync(Guid teamId, Guid userId, FunctionalRole minimumRole, CancellationToken ct)
    {
        return _db.TeamMembers.AnyAsync(m =>
                m.TeamId == teamId &&
                m.UserId == userId &&
                m.Role >= minimumRole,
            ct);
    }

    public Task<TeamMember?> GetByTeamAndUserAsync(Guid teamId, Guid userId, CancellationToken ct)
    {
        return _db.TeamMembers
            .Include(m => m.User)
            .FirstOrDefaultAsync(m => m.TeamId == teamId && m.UserId == userId, ct);
    }

    public Task<List<TeamMember>> GetLeadersAsync(Guid teamId, CancellationToken ct)
    {
        return _db.TeamMembers
            .Where(m => m.TeamId == teamId && m.Role == FunctionalRole.Leader)
            .ToListAsync(ct);
    }

    public async Task AddAsync(TeamMember member, CancellationToken ct) => await _db.TeamMembers.AddAsync(member, ct);
    public void Remove(TeamMember member) => _db.TeamMembers.Remove(member);
}