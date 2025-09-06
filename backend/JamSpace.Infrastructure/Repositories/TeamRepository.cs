using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Domain.Entities;
using JamSpace.Domain.Enums;
using JamSpace.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace JamSpace.Infrastructure.Repositories;

public class TeamRepository : ITeamRepository
{
    private readonly JamSpaceDbContext _db;

    public TeamRepository(JamSpaceDbContext db) => _db = db;

    public async Task<Guid> CreateTeamAsync(Team team, Guid creatorUserId, CancellationToken ct)
    {
        await _db.Teams.AddAsync(team);
        await _db.TeamMembers.AddAsync(new TeamMember
        {
            Team = team,
            UserId = creatorUserId,
            Role = FunctionalRole.Leader
        });
        await _db.SaveChangesAsync(ct);
        return team.Id;
    }

    public async Task<Team?> GetTeamByIdAsync(Guid id, CancellationToken ct)
    {
        var team = await _db.Teams
            .Include(t => t.CreatedBy)
            .Include(t => t.Members)
            .ThenInclude(m => m.User)
            .FirstOrDefaultAsync(t => t.Id == id, ct);

        if (team is null)
            throw new NotFoundException("Team not found.");
        
        team.Members = team.Members
            .OrderBy(m => m.Role == FunctionalRole.Leader ? 0
                : m.Role == FunctionalRole.Admin  ? 1
                : 2)
            .ThenBy(m => m.User.UserName)
            .ToList();

        return team;
    }

    public async Task<List<Team>> GetTeamsByUserIdAsync(Guid userId, CancellationToken ct)
    {
        return await _db.Teams
            .Where(t => t.Members.Any(m => m.UserId == userId))
            .Include(t => t.CreatedBy)
            .Include(t => t.Members)
            .ThenInclude(m => m.User)
            .ToListAsync(ct);
    }

    public async Task<Team> ChangeTeamNameAsync(Guid teamId, string name, CancellationToken ct)
    {
        var team = await GetTeamByIdAsync(teamId, ct);
        team!.Name = name;
        await _db.SaveChangesAsync(ct);
        return team;
    }
    
    public async Task UpdateTeamPictureAsync(
        Guid teamId, Guid requestingUserId, string pictureUrl, CancellationToken ct)
    {
        var team = await GetTeamByIdAsync(teamId, ct);
        if (!await _db.TeamMembers.AnyAsync(m => m.TeamId == teamId && m.UserId == requestingUserId && 
                                                 (m.Role == FunctionalRole.Leader || m.Role == FunctionalRole.Admin)))
            throw new ForbiddenAccessException("Only team leader or admin can update team picture.");

        team!.TeamPictureUrl = pictureUrl;
        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteTeamAsync(Guid teamId, CancellationToken ct)
    {
        var team = await GetTeamByIdAsync(teamId, ct);
        _db.Teams.Remove(team!);
        await _db.SaveChangesAsync(ct);
    }
}