using JamSpace.Application.Common.Interfaces;
using JamSpace.Domain.Entities;
using JamSpace.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace JamSpace.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly JamSpaceDbContext _db;
    public UserRepository(JamSpaceDbContext db) => _db = db;

    public async Task<bool> ExistsAsync(Guid id, CancellationToken ct)
    {
        return await _db.Users.AnyAsync(u => u.Id == id);
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await _db.Users
            .Include(u => u.Followers)
            .Include(u => u.Following)
            .Include(u => u.UserSkills)
            .ThenInclude(us => us.Skill)
            .Include(u => u.UserGenres)
            .ThenInclude(ug => ug.Genre) 
            .FirstOrDefaultAsync(u => u.Id == id, ct);
    }
        
    public async Task<User?> GetByEmailAsync(string email, CancellationToken ct) =>
        await _db.Users.FirstOrDefaultAsync(u => u.Email == email, ct); 

    public async Task<Guid?> GetUserIdByUsernameAsync(string username, CancellationToken ct)
    {
        var normalized = username.Trim();
        return await _db.Users
            .AsNoTracking()
            .Where(u => u.UserName == normalized)
            .Select(u => (Guid?)u.Id)
            .FirstOrDefaultAsync(ct);
    }

    public async Task AddAsync(User user, CancellationToken ct) => await _db.Users.AddAsync(user, ct);
    
}