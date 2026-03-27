using JamSpace.Application.Common.Interfaces;
using JamSpace.Domain.Common;
using JamSpace.Domain.Entities;
using JamSpace.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace JamSpace.Infrastructure.Repositories;

public class SkillRepository : ISkillRepository
{
    private readonly JamSpaceDbContext _db;

    public SkillRepository(JamSpaceDbContext db)
    {
        _db = db;
    }

    public async Task<Skill?> GetSkillByIdAsync(Guid skillId, CancellationToken ct)
    {
        return await _db.Skills.FirstOrDefaultAsync(s => s.Id == skillId, ct);
    }

    public async Task<Skill?> GetSkillByNameAsync(string name, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(name))
            return null;

        var normalized = NameConventions.NormalizeForQuery(name);

        return await _db.Skills.FirstOrDefaultAsync(
            s => s.Name.Replace(" ", "").Replace("-", "").Replace("_", "").ToLower() == normalized,
            ct);
    }

    public async Task AddAsync(Skill skill, CancellationToken ct)
    {
        await _db.Skills.AddAsync(skill, ct);
    }
}