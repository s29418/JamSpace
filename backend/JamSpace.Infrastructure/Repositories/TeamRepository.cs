using DefaultNamespace;
using JamSpace.Application.Interfaces;
using JamSpace.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace JamSpace.Infrastructure.Repositories;

public class TeamRepository : ITeamRepository
{
    private readonly JamSpaceDbContext _db;
    
    public TeamRepository(JamSpaceDbContext db)
    {
        _db = db;
    }
    
    public async Task<Guid> CreateTeamAsync(Team team, Guid creatorUserId)
    {
        _db.Teams.Add(team);
        _db.TeamMembers.Add(new TeamMember
        {
            Team = team,
            UserId = creatorUserId,
            Role = "Owner"
        });

        await _db.SaveChangesAsync();

        return team.Id;
    }


    public async Task<Team?> GetTeamByIdAsync(Guid id)
    {
        return await _db.Teams
            .Include(t => t.CreatedBy)
            .Include(t => t.Members)
                .ThenInclude(m => m.User)
            .FirstOrDefaultAsync(t => t.Id == id);
    }
    
    public async Task<bool> IsUserInTeamAsync(Guid teamId, Guid userId)
    {
        return await _db.TeamMembers.AnyAsync(m => m.TeamId == teamId && m.UserId == userId);
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

    public async Task SendTeamInviteAsync(Guid teamId, Guid invitedUserId, Guid invitedByUserId, CancellationToken ct)
    {
        var alreadyExists = await _db.TeamInvites.AnyAsync(
            i => i.TeamId == teamId 
                 && i.InvitedUserId == invitedUserId 
                 && i.Status == InviteStatus.Pending,
            ct);

        if (alreadyExists)
            return;

        var invite = new TeamInvite
        {
            Id = Guid.NewGuid(),
            TeamId = teamId,
            InvitedUserId = invitedUserId,
            InvitedByUserId = invitedByUserId,
            CreatedAt = DateTime.UtcNow,
            Status = InviteStatus.Pending
        };

        _db.TeamInvites.Add(invite);
        await _db.SaveChangesAsync(ct);
    }

}