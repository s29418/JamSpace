using JamSpace.Domain.Entities;

namespace JamSpace.Application.Common.Interfaces;

public interface ITeamInviteRepository
{
    Task<TeamInvite?> GetByIdAsync(Guid inviteId, CancellationToken ct);
    Task<TeamInvite?> GetByIdWithDetailsAsync(Guid inviteId, CancellationToken ct);

    Task<bool> ExistsPendingInviteAsync(Guid teamId, Guid invitedUserId, CancellationToken ct);
    Task<bool> WasInviteSentByUserAsync(Guid inviteId, Guid userId, CancellationToken ct);

    Task<List<TeamInvite>> GetPendingInvitesForTeamAsync(Guid teamId, CancellationToken ct);
    Task<List<TeamInvite>> GetPendingInvitesForTeamSentByUserAsync(Guid teamId, Guid invitedByUserId, CancellationToken ct);
    Task<List<TeamInvite>> GetMyPendingInvitesAsync(Guid userId, CancellationToken ct);

    Task AddAsync(TeamInvite invite, CancellationToken ct);
    void Remove(TeamInvite invite);
}