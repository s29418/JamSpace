using DefaultNamespace;

namespace JamSpace.Application.Interfaces;

public interface ITeamRepository
{
    Task<Guid> CreateTeamAsync(Team team, Guid creatorUserId);
    Task<Team?> GetTeamByIdAsync(Guid id);
    Task InviteUserAsync(Guid teamId, Guid invitedUserId);
}
