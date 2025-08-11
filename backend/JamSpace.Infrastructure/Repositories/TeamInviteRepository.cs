using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Domain.Entities;
using JamSpace.Domain.Enums;
using JamSpace.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace JamSpace.Infrastructure.Repositories;

public class TeamInviteRepository : ITeamInviteRepository
{
    private readonly JamSpaceDbContext _db;
    private readonly ITeamMemberRepository _memberRepo;

    public TeamInviteRepository(JamSpaceDbContext db, ITeamMemberRepository memberRepo)
    {
        _db = db;
        _memberRepo = memberRepo;
    }
    
    public async Task<TeamInvite> SendTeamInviteAsync(Guid teamId, Guid invitedUserId, Guid invitedByUserId, CancellationToken ct)
    {
        var alreadyExists = await _db.TeamInvites
            .AnyAsync(i => i.TeamId == teamId && i.InvitedUserId == invitedUserId && i.Status == InviteStatus.Pending, ct);
        if (alreadyExists || await _memberRepo.IsUserInTeamAsync(teamId, invitedUserId))
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

        await _db.TeamInvites.AddAsync(invite);
        await _db.SaveChangesAsync(ct);
        
        await _db.Entry(invite).Reference(i => i.Team).LoadAsync(ct);
        await _db.Entry(invite).Reference(i => i.InvitedUser).LoadAsync(ct);
        await _db.Entry(invite).Reference(i => i.InvitedByUser).LoadAsync(ct);
        
        return invite;
    }
    
    public async Task<TeamInvite> GetTeamInviteByIdAsync(Guid teamInviteId, CancellationToken ct)
    {
        var invite = await _db.TeamInvites
                         .Include(i => i.Team)
                         .Include(i => i.InvitedByUser)
                         .Include(i => i.InvitedUser)
                         .SingleOrDefaultAsync(i => i.Id == teamInviteId, ct)
                     ?? throw new NotFoundException("Invite not found.");
        
        return invite;
    }
    
    public async Task<List<TeamInvite>> GetTeamInvitesAsync(Guid teamId, Guid requestingUserId, CancellationToken ct)
    {
        var query = _db.TeamInvites
            .Where(i => i.TeamId == teamId && i.Status == InviteStatus.Pending)
            .Include(i => i.Team)
            .Include(i => i.InvitedByUser)
            .Include(i => i.InvitedUser);

        if (await _memberRepo.IsUserALeaderAsync(teamId, requestingUserId) || await _memberRepo.IsUserAnAdminAsync(teamId, requestingUserId))
            return await query.ToListAsync(ct);

        return await query.Where(i => i.InvitedByUserId == requestingUserId).ToListAsync(ct);
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

    public async Task<bool> WasInviteSentByUserAsync(Guid inviteId, Guid userId, CancellationToken ct)
    {
        return await _db.TeamInvites.AnyAsync(i => i.Id == inviteId && i.InvitedByUserId == userId, ct);
    }

    public async Task<TeamInvite> AcceptInviteAsync(Guid inviteId, Guid userId, CancellationToken ct)
    {
        var invite = await GetTeamInviteByIdAsync(inviteId, ct);
        
        if (invite.Status != InviteStatus.Pending)
            throw new InvalidOperationException("Invite is no longer pending.");

        invite.Status = InviteStatus.Accepted;
        await _db.TeamMembers.AddAsync(new TeamMember { TeamId = invite.TeamId, UserId = userId });
        await _db.SaveChangesAsync(ct);
        
        return invite;
    }

    public async Task<TeamInvite> RejectInviteAsync(Guid inviteId, Guid userId, CancellationToken ct)
    {
        var invite = await GetTeamInviteByIdAsync(inviteId, ct);
        if (invite.Status != InviteStatus.Pending)
            throw new InvalidOperationException("Invite is no longer pending.");

        invite.Status = InviteStatus.Rejected;
        await _db.SaveChangesAsync(ct);
        
        return invite;
    }

    public async Task<TeamInvite> CancelTeamInviteAsync(Guid inviteId, Guid requestingUserId, CancellationToken ct)
    {
        var invite = await GetTeamInviteByIdAsync(inviteId, ct);

        invite.Status = InviteStatus.Cancelled;
        await _db.SaveChangesAsync(ct);
        return invite;
    }
}