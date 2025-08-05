using JamSpace.Application.Common.Exceptions;
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

    public async Task<bool> IsUserInTeamAsync(Guid teamId, Guid userId)
    {
        return await _db.TeamMembers.AnyAsync(m => m.TeamId == teamId && m.UserId == userId);
    }

    public async Task<bool> IsUserALeaderAsync(Guid teamId, Guid userId)
    {
        return await _db.TeamMembers.AnyAsync(m => m.TeamId == teamId && m.UserId == userId && m.Role == FunctionalRole.Leader);
    }

    public async Task<bool> IsUserAnAdminAsync(Guid teamId, Guid userId)
    {
        return await _db.TeamMembers.AnyAsync(m => m.TeamId == teamId && m.UserId == userId && m.Role == FunctionalRole.Admin);
    }

    public async Task<TeamMember> GetTeamMemberAsync(Guid teamId, Guid userId, CancellationToken ct)
    {
        var member = await _db.TeamMembers.Include(m => m.User).FirstOrDefaultAsync(m => m.TeamId == teamId && m.UserId == userId, ct);
        if (member is null)
            throw new NotFoundException("Team member not found.");
        return member;
    }

    public async Task<List<TeamMember>> GetLeadersAsync(Guid teamId, CancellationToken ct)
    {
        return await _db.TeamMembers.Where(m => m.TeamId == teamId && m.Role == FunctionalRole.Leader).ToListAsync(ct);
    }

    public async Task<TeamMember> ChangeTeamMemberFunctionalRoleAsync(Guid teamId, Guid userId, FunctionalRole newRole, CancellationToken ct)
    {
        var member = await GetTeamMemberAsync(teamId, userId, ct);
        member.Role = newRole;
        await _db.SaveChangesAsync(ct);
        return member;
    }

    public async Task<TeamMember> EditTeamMemberMusicalRole(Guid teamId, Guid userId, string musicalRole, CancellationToken ct)
    {
        var member = await GetTeamMemberAsync(teamId, userId, ct);
        member.MusicalRole = musicalRole;
        await _db.SaveChangesAsync(ct);
        return member;
    }

    public async Task DeleteTeamMemberAsync(Guid teamId, Guid userId, CancellationToken ct)
    {
        var member = await GetTeamMemberAsync(teamId, userId, ct);
        _db.TeamMembers.Remove(member);
        await _db.SaveChangesAsync(ct);
    }
}