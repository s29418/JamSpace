using DefaultNamespace;
using JamSpace.Application.Common.Exceptions;
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
        await _db.Teams.AddAsync(team);
        await _db.TeamMembers.AddAsync(new TeamMember
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
    
    public async Task<Guid?> GetUserIdByUsernameAsync(string username, CancellationToken ct)
    {
        var user = await _db.Users
            .Where(u => u.UserName == username)
            .Select(u => new { u.Id })
            .FirstOrDefaultAsync(ct);

        return user?.Id;
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
    
    public async Task<List<TeamInvite>> GetMyPendingInvitesAsync(Guid userId, CancellationToken ct)
    {
        return await _db.TeamInvites
            .Where(i => i.InvitedUserId == userId && i.Status == InviteStatus.Pending)
            .Include(i => i.Team)
            .Include(i => i.InvitedByUser)
            .ToListAsync(ct);
    }

    public async Task AcceptInviteAsync(Guid inviteId, Guid userId, CancellationToken ct)
    {
        var invite = await _db.TeamInvites
            .Include(i => i.Team)
            .FirstOrDefaultAsync(i => i.Id == inviteId && i.InvitedUserId == userId, ct);

        if (invite is null)
            throw new NotFoundException("Invite not found.");

        if (invite.Status != InviteStatus.Pending)
            throw new InvalidOperationException("Invite is no longer pending.");

        invite.Status = InviteStatus.Accepted;

        _db.TeamMembers.Add(new TeamMember
        {
            TeamId = invite.TeamId,
            UserId = userId
        });

        await _db.SaveChangesAsync(ct);
    }

    public async Task RejectInviteAsync(Guid inviteId, Guid userId, CancellationToken ct)
    {
        var invite = await _db.TeamInvites
            .FirstOrDefaultAsync(i => i.Id == inviteId && i.InvitedUserId == userId, ct);

        if (invite is null)
            throw new NotFoundException("Invite not found.");

        if (invite.Status != InviteStatus.Pending)
            throw new InvalidOperationException("Invite is no longer pending.");

        invite.Status = InviteStatus.Rejected;
        await _db.SaveChangesAsync(ct);
    }


}