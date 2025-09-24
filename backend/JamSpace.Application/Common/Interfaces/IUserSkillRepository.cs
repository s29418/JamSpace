using JamSpace.Domain.Entities;

namespace JamSpace.Application.Common.Interfaces;

public interface IUserSkillRepository
{
    Task<bool> UserHasSkillAsync(Guid userId, Guid genreId, CancellationToken ct);
    Task<List<UserSkill>> GetAllUserSkillsAsync(Guid userId, CancellationToken ct);
    Task<UserSkill> AddUserSkillAsync(Guid userId, Guid genreId, CancellationToken ct);
    Task RemoveUserSkillAsync(Guid userId, Guid genreId, CancellationToken ct);
}