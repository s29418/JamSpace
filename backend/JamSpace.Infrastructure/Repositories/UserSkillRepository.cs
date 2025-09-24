using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Domain.Entities;
using JamSpace.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace JamSpace.Infrastructure.Repositories;

public class UserSkillRepository : IUserSkillRepository
{
    private readonly JamSpaceDbContext _db;
    
    public UserSkillRepository(JamSpaceDbContext db)
    {
        _db = db;
    }
    
    
    public async Task<bool> UserHasSkillAsync(Guid userId, Guid skillId, CancellationToken ct)
    {
        return await _db.UserSkills.AnyAsync(us => us.SkillId == skillId && us.UserId == userId);
    }

    public async Task<List<UserSkill>> GetAllUserSkillsAsync(Guid userId, CancellationToken ct)
    {
        return await _db.UserSkills
            .Where(us => us.UserId == userId)
            .OrderByDescending(us => us.AddeddAt)
            .Include(us => us.Skill)
            .ToListAsync(ct);
    }

    public async Task<UserSkill> AddUserSkillAsync(Guid userId, Guid skillId, CancellationToken ct)
    {
        var userSkill = new UserSkill
        {
            UserId = userId,
            SkillId = skillId,
            AddeddAt = DateTime.UtcNow
        };
        
        _db.UserSkills.Add(userSkill);
        await _db.SaveChangesAsync(ct);
        return userSkill;
    }

    public async Task RemoveUserSkillAsync(Guid userId, Guid skillId, CancellationToken ct)
    {
        var userSkill = await _db.UserSkills
            .FirstOrDefaultAsync(ug => ug.UserId == userId && ug.SkillId == skillId, ct);

        if (userSkill == null)
            throw new NotFoundException("UserSkill not found.");
        
        _db.UserSkills.Remove(userSkill);
        await _db.SaveChangesAsync(ct);
    }
}