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
        return await _db.UserSkills.AnyAsync(us => us.SkillId == skillId && us.UserId == userId, ct);
    }

    public async Task<List<UserSkill>> GetAllUserSkillsAsync(Guid userId, CancellationToken ct)
    {
        return await _db.UserSkills
            .Where(us => us.UserId == userId)
            .OrderByDescending(us => us.AddeddAt)
            .Include(us => us.Skill)
            .ToListAsync(ct);
    }

    public async Task<UserSkill?> GetUserSkillAsync(Guid userId, Guid skillId, CancellationToken ct)
    {
        return await _db.UserSkills
            .FirstOrDefaultAsync(us => us.UserId == userId && us.SkillId == skillId, ct);
    }

    public async Task AddAsync(UserSkill userSkill, CancellationToken ct)
    {
        await _db.UserSkills.AddAsync(userSkill, ct);
    }

    public void Remove(UserSkill userSkill)
    {
        _db.UserSkills.Remove(userSkill);
    }
}