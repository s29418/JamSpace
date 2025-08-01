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
            Role = FunctionalRole.Leader
        });

        await _db.SaveChangesAsync();

        return team.Id;
    }

    public async Task<Team?> GetTeamByIdAsync(Guid id)
    {
        var team = await _db.Teams
            .Include(t => t.CreatedBy)
            .Include(t => t.Members)
                .ThenInclude(m => m.User)
            .FirstOrDefaultAsync(t => t.Id == id);
        
        if (team is null)
            throw new NotFoundException("Team not found.");

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
        var team = await GetTeamByIdAsync(teamId);
        team!.Name = name;
        await _db.SaveChangesAsync(ct);
        return team;
    }

    public async Task DeleteTeamAsync(Guid teamId, CancellationToken ct)
    {
        var team = await GetTeamByIdAsync(teamId);
        _db.Teams.Remove(team!);
        await _db.SaveChangesAsync(ct);
    }

    public async Task SendTeamInviteAsync(Guid teamId, Guid invitedUserId, Guid invitedByUserId, CancellationToken ct)
    {
        var alreadyExists = await _db.TeamInvites.AnyAsync(
            i => i.TeamId == teamId 
                 && i.InvitedUserId == invitedUserId 
                 && i.Status == InviteStatus.Pending,
            ct);

        if (alreadyExists || await IsUserInTeamAsync(teamId, invitedUserId))
            throw new ConflictException("Invite already exists or user is in the team.");

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
            .Include(i => i.InvitedUser)
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

    public async Task<TeamInvite> GetTeamInviteByIdAsync(Guid teamInviteId, CancellationToken ct)
    {
        return await _db.TeamInvites
            .Include(i => i.Team)
            .Include(i => i.InvitedByUser)
            .Include(i => i.InvitedUser)
            .FirstOrDefaultAsync(i => i.Id == teamInviteId, ct);
    }

    public async Task<bool> WasInviteSentByUserAsync(Guid inviteId, Guid userId, CancellationToken ct)
    {
        return await _db.TeamInvites
            .AnyAsync(i => i.Id == inviteId && i.InvitedByUserId == userId, ct);   
    }

    public async Task<List<TeamInvite>> GetTeamInvitesAsync(Guid teamId, Guid requestingUserId, CancellationToken ct)
    {
        var query = _db.TeamInvites
            .Where(i => i.TeamId == teamId && i.Status == InviteStatus.Pending)
            .Include(i => i.Team)
            .Include(i => i.InvitedByUser)
            .Include(i => i.InvitedUser);

        if (await IsUserALeaderAsync(teamId, requestingUserId) || await IsUserAnAdminAsync(teamId, requestingUserId))
        {
            return await query.ToListAsync(ct); 
        }

        return await query
            .Where(i => i.InvitedByUserId == requestingUserId)
            .ToListAsync(ct); 
    }

    public async Task CancelTeamInviteAsync(Guid inviteId, Guid requestingUserId, CancellationToken ct)
    {
        var invite = await GetTeamInviteByIdAsync(inviteId, ct);

        if (invite is null)
            throw new NotFoundException("Invite not found.");

        var teamId = invite.TeamId;

        var hasPermission =
            await IsUserALeaderAsync(teamId, requestingUserId) ||
            await IsUserAnAdminAsync(teamId, requestingUserId) ||
            await WasInviteSentByUserAsync(inviteId, requestingUserId, ct);

        if (!hasPermission)
            throw new ForbiddenAccessException("Only team leader, admin or user who sent the invite can cancel it.");

        invite.Status = InviteStatus.Cancelled;
        await _db.SaveChangesAsync(ct);
    }

    
    public async Task<bool> IsUserInTeamAsync(Guid teamId, Guid userId)
    {
        return await _db.TeamMembers.AnyAsync(m => m.TeamId == teamId && m.UserId == userId);
    }

    public async Task<bool> IsUserALeaderAsync(Guid teamId, Guid userId)
    {
        return await _db.TeamMembers.AnyAsync(m =>
            m.TeamId == teamId && m.UserId == userId && m.Role == FunctionalRole.Leader);
    }

    public async Task<bool> IsUserAnAdminAsync(Guid teamId, Guid userId)
    {
        return await _db.TeamMembers
            .AnyAsync(m => m.TeamId == teamId && m.UserId == userId && m.Role == FunctionalRole.Admin);
    }

    public async Task<TeamMember> GetTeamMemberAsync(Guid teamId, Guid userId, CancellationToken ct)
    {
        var member = await _db.TeamMembers
            .Include(m => m.User)
            .FirstOrDefaultAsync(m => m.TeamId == teamId && m.UserId == userId, ct);

        if (member is null)
            throw new NotFoundException("Team member not found.");

        return member;
    }

    public async Task<List<TeamMember>> GetLeadersAsync(Guid teamId, CancellationToken ct)
    {
        return await _db.TeamMembers
            .Where(m => m.TeamId == teamId && m.Role == FunctionalRole.Leader)
            .ToListAsync(ct);
    }
    
    public async Task<TeamMember> ChangeTeamMemberFunctionalRoleAsync(
        Guid teamId, Guid userId, FunctionalRole newRole, CancellationToken ct)
    {
        var teamMember = await GetTeamMemberAsync(teamId, userId, ct);

        teamMember.Role = newRole;

        await _db.SaveChangesAsync(ct);
        return teamMember;
    }

    public async Task KickTeamMemberAsync(Guid teamId, Guid userId, CancellationToken ct)
    {
        var teamMember = await GetTeamMemberAsync(teamId, userId, ct);
        
        _db.TeamMembers.Remove(teamMember);
        await _db.SaveChangesAsync(ct);
    }

    public async Task<TeamMember> EditTeamMemberMusicalRole(Guid teamId, Guid userId, string musicalRole, CancellationToken ct)
    {
        var teamMember = await GetTeamMemberAsync(teamId, userId, ct);
        
        teamMember.MusicalRole = musicalRole;
        await _db.SaveChangesAsync(ct);
        return teamMember;
    }

    public async Task UpdateTeamPictureAsync(Guid teamId, Guid requestingUserId, string pictureUrl,
        CancellationToken ct)
    {
        var team = await GetTeamByIdAsync(teamId);
        
        if (!await IsUserALeaderAsync(teamId, requestingUserId) && !await IsUserAnAdminAsync(teamId, requestingUserId))
            throw new ForbiddenAccessException("Only team leader or admin can update team picture.");

        team.TeamPictureUrl = pictureUrl;
        await _db.SaveChangesAsync(ct);
    }

}