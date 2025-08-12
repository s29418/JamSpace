using JamSpace.Domain.Entities;
namespace JamSpace.Application.Common.Interfaces;

public interface ITeamRepository
{
    Task<Guid> CreateTeamAsync(Team team, Guid creatorUserId);
    Task<Team?> GetTeamByIdAsync(Guid id);
    Task<List<Team>> GetTeamsByUserIdAsync(Guid userId, CancellationToken ct);
    Task<Team> ChangeTeamNameAsync(Guid teamId, string name, CancellationToken ct);
    Task UpdateTeamPictureAsync(Guid teamId, Guid requestingUserId, string pictureUrl, CancellationToken ct);
    Task DeleteTeamAsync(Guid teamId, CancellationToken ct);
}
