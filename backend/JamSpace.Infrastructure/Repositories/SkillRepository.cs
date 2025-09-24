using JamSpace.Application.Common.Exceptions;
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
        
        
    public async Task<Skill> GetSkillByIdAsync(Guid skillId, CancellationToken ct)
    {
        var skill = await _db.Skills.FirstOrDefaultAsync(s => s.Id == skillId, ct) 
                    ?? throw new NotFoundException("Skill not found.");
        
        return skill;
    }

    public async Task<Skill?> GetSkillByNameAsync(string name, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(name))
            return null;

        var normalized = NameConventions.NormalizeForQuery(name);
        return await _db.Skills.FirstOrDefaultAsync(
            s => s.Name.Replace(" ", "").Replace("-", "").Replace("_", "").ToLower() == normalized, ct);
    }

    public async Task<Skill> CreateSkillAsync(string skillName, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(skillName))
            throw new ArgumentException("Skill name is required.", nameof(skillName));
        
        var pretty = NameConventions.PrettifyForDisplay(skillName);

        var skill = new Skill
        {
            Id = Guid.NewGuid(),
            Name = pretty
        };
        
        _db.Skills.Add(skill);
        await _db.SaveChangesAsync(ct);
        return skill;
    }
}