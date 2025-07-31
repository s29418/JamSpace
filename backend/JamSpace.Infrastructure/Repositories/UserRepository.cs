using JamSpace.Application.Common.Interfaces;
using JamSpace.Domain.Entities;
using JamSpace.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace JamSpace.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly JamSpaceDbContext _db;

    public UserRepository(JamSpaceDbContext db)
    {
        _db = db;
    }

    public async Task<User?> GetByEmailAsync(string email) =>
        await _db.Users.FirstOrDefaultAsync(u => u.Email == email);

    public async Task AddAsync(User user)
    {
        _db.Users.Add(user);
        await _db.SaveChangesAsync();
    }
    
    public async Task<Guid?> GetUserIdByUsernameAsync(string username, CancellationToken ct)
    {
        var user = await _db.Users
            .Where(u => u.UserName == username)
            .Select(u => new { u.Id })
            .FirstOrDefaultAsync(ct);

        return user?.Id;
    }
}