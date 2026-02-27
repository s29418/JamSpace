using JamSpace.Application.Common.Interfaces;
using JamSpace.Domain.Entities;
using JamSpace.Domain.Enums;
using JamSpace.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace JamSpace.Infrastructure.Repositories;

public class TeamInviteRepository : ITeamInviteRepository
{
    private readonly JamSpaceDbContext _db;

    public TeamInviteRepository(JamSpaceDbContext db)
    {
        _db = db;
    }

    public async Task<TeamInvite?> GetByIdAsync(Guid inviteId, CancellationToken ct)
    {
        return await _db.TeamInvites
            .SingleOrDefaultAsync(i => i.Id == inviteId, ct);
    }

    public async Task<TeamInvite?> GetByIdWithDetailsAsync(Guid inviteId, CancellationToken ct)
    {
        return await _db.TeamInvites
            .Include(i => i.Team)
            .Include(i => i.InvitedByUser)
            .Include(i => i.InvitedUser)
            .SingleOrDefaultAsync(i => i.Id == inviteId, ct);
    }

    public async Task<bool> ExistsPendingInviteAsync(Guid teamId, Guid invitedUserId, CancellationToken ct)
    {
        return await _db.TeamInvites.AnyAsync(
            i => i.TeamId == teamId &&
                 i.InvitedUserId == invitedUserId &&
                 i.Status == InviteStatus.Pending,
            ct);
    }

    public async Task<bool> WasInviteSentByUserAsync(Guid inviteId, Guid userId, CancellationToken ct)
    {
        return await _db.TeamInvites.AnyAsync(i => i.Id == inviteId && i.InvitedByUserId == userId, ct);
    }

    public async Task<List<TeamInvite>> GetPendingInvitesForTeamAsync(Guid teamId, CancellationToken ct)
    {
        return await _db.TeamInvites
            .Where(i => i.TeamId == teamId && i.Status == InviteStatus.Pending)
            .Include(i => i.Team)
            .Include(i => i.InvitedByUser)
            .Include(i => i.InvitedUser)
            .ToListAsync(ct);
    }

    public async Task<List<TeamInvite>> GetPendingInvitesForTeamSentByUserAsync(Guid teamId, Guid invitedByUserId, CancellationToken ct)
    {
        return await _db.TeamInvites
            .Where(i => i.TeamId == teamId &&
                        i.InvitedByUserId == invitedByUserId &&
                        i.Status == InviteStatus.Pending)
            .Include(i => i.Team)
            .Include(i => i.InvitedByUser)
            .Include(i => i.InvitedUser)
            .ToListAsync(ct);
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

    public async Task AddAsync(TeamInvite invite, CancellationToken ct)
    {
        await _db.TeamInvites.AddAsync(invite, ct);
    }

    public void Remove(TeamInvite invite)
    {
        _db.TeamInvites.Remove(invite);
    }
}