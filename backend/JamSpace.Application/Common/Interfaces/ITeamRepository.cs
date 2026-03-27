using JamSpace.Domain.Entities;
namespace JamSpace.Application.Common.Interfaces;

public interface ITeamRepository
{
    Task<Team?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<List<Team>> GetByUserIdAsync(Guid userId, CancellationToken ct);
    
    Task AddAsync(Team team,  CancellationToken ct);
    void Remove(Team team);
}
