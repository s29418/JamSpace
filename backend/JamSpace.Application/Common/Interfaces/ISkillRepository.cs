using JamSpace.Domain.Entities;

namespace JamSpace.Application.Common.Interfaces;

public interface ISkillRepository
{
    Task<Skill?> GetSkillByIdAsync(Guid skillId, CancellationToken ct);
    Task<Skill?> GetSkillByNameAsync(string skillName, CancellationToken ct);
    Task AddAsync(Skill skill, CancellationToken ct);
}