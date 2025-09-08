using JamSpace.Domain.Entities;

namespace JamSpace.Application.Common.Interfaces;

public interface ITeamInviteRepository
{
    Task<TeamInvite> SendTeamInviteAsync(Guid teamId, Guid invitedUserId, Guid invitedByUserId, CancellationToken ct);
    Task<TeamInvite> GetTeamInviteByIdAsync(Guid teamInviteId, CancellationToken ct);
    Task<List<TeamInvite>> GetTeamInvitesAsync(Guid teamId, Guid requestingUserId, CancellationToken ct);
    Task<List<TeamInvite>> GetMyPendingInvitesAsync(Guid userId, CancellationToken ct);
    Task<bool> WasInviteSentByUserAsync(Guid inviteId, Guid userId, CancellationToken ct);
    Task<TeamInvite> AcceptInviteAsync(Guid inviteId, Guid userId, CancellationToken ct);
    Task<TeamInvite> RejectInviteAsync(Guid inviteId, Guid userId, CancellationToken ct);
    Task<TeamInvite> CancelTeamInviteAsync(Guid inviteId, Guid requestingUserId, CancellationToken ct);
}