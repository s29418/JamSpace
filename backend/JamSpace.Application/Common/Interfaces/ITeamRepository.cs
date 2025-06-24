using DefaultNamespace;

namespace JamSpace.Application.Interfaces;

public interface ITeamRepository
{
    Task<Guid> CreateTeamAsync(Team team, Guid creatorUserId);
    Task<Team?> GetTeamByIdAsync(Guid id);
    Task<bool> IsUserInTeamAsync(Guid teamId, Guid userId);
    Task<List<Team>> GetTeamsByUserIdAsync(Guid userId, CancellationToken ct);
    Task SendTeamInviteAsync(Guid teamId, Guid invitedUserId, Guid invitedByUserId, CancellationToken ct);
    Task<List<TeamInvite>> GetMyPendingInvitesAsync(Guid userId, CancellationToken ct);
    Task AcceptInviteAsync(Guid inviteId, Guid userId, CancellationToken ct);
    Task RejectInviteAsync(Guid inviteId, Guid userId, CancellationToken ct);

}
