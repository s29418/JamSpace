using JamSpace.Domain.Entities;

namespace JamSpace.Application.Common.Interfaces;

public interface IUserSkillRepository
{
    Task<bool> UserHasSkillAsync(Guid userId, Guid skillId, CancellationToken ct);
    Task<List<UserSkill>> GetAllUserSkillsAsync(Guid userId, CancellationToken ct);
    Task<UserSkill?> GetUserSkillAsync(Guid userId, Guid skillId, CancellationToken ct);
    Task AddAsync(UserSkill userSkill, CancellationToken ct);
    public void Remove(UserSkill userSkill);
}