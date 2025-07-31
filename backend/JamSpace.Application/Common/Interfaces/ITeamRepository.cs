using DefaultNamespace;
using JamSpace.Domain.Enums;

namespace JamSpace.Application.Common.Interfaces;

public interface ITeamRepository
{
    //Team management
    Task<Team?> GetTeamByIdAsync(Guid id);
    Task<List<Team>> GetTeamsByUserIdAsync(Guid userId, CancellationToken ct);
    Task<Guid> CreateTeamAsync(Team team, Guid creatorUserId);

    Task<bool> IsUserInTeamAsync(Guid teamId, Guid userId);
    Task<Guid?> GetUserIdByUsernameAsync(string username, CancellationToken ct);
    
    //Team invites
    Task<List<TeamInvite>> GetMyPendingInvitesAsync(Guid userId, CancellationToken ct);
    Task SendTeamInviteAsync(Guid teamId, Guid invitedUserId, Guid invitedByUserId, CancellationToken ct);
    Task AcceptInviteAsync(Guid inviteId, Guid userId, CancellationToken ct);
    Task RejectInviteAsync(Guid inviteId, Guid userId, CancellationToken ct);
    
    //Team members functional roles
    Task<bool> IsUserALeaderAsync(Guid teamId, Guid userId);
    Task<bool> IsUserAnAdminAsync(Guid teamId, Guid userId);
    Task<TeamMember> GetTeamMemberAsync(Guid teamId, Guid userId, CancellationToken ct);
    Task<List<TeamMember>> GetLeadersAsync(Guid teamId, CancellationToken ct);
    Task<TeamMember> ChangeTeamMemberFunctionalRoleAsync(
        Guid teamId, Guid userId, FunctionalRole newRole, CancellationToken ct);


    Task<Team> ChangeTeamNameAsync(Guid teamId, string name, CancellationToken ct);
    Task DeleteTeamAsync(Guid teamId, CancellationToken ct);
    // Task UpdateTeamPictureAsync(Guid teamId, string pictureUrl, CancellationToken ct);
    Task KickTeamMemberAsync(Guid teamId, Guid userId, CancellationToken ct);
    Task<TeamMember> EditTeamMemberMusicalRole(Guid teamId, Guid userId, string musicalRole, CancellationToken ct);

    // Task<List<TeamInvite>> GetTeamInvitesAsync(Guid teamId, Guid requestingUserId, CancellationToken ct);
    // Task CancelInviteAsync(Guid inviteId, Guid requestingUserId, CancellationToken ct);
    // Task<bool> WasInviteSentByUserAsync(Guid inviteId, Guid userId, CancellationToken ct);

}
