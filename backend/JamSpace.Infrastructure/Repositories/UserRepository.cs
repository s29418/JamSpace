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

    public async Task<User?> GetByEmailAsync(string email, CancellationToken ct) =>
        await _db.Users.FirstOrDefaultAsync(u => u.Email == email, ct);

    public async Task AddAsync(User user, CancellationToken ct)
    {
        _db.Users.Add(user);
        await _db.SaveChangesAsync(ct);
    }
    
    public async Task<Guid?> GetUserIdByUsernameAsync(string username, CancellationToken ct)
    {
        var user = await _db.Users
            .Where(u => u.UserName == username)
            .Select(u => new { u.Id })
            .FirstOrDefaultAsync(ct);

        return user?.Id;
    }

    public async Task<bool> ExistsAsync(Guid userId, CancellationToken ct)
    {
        return await _db.Users.AnyAsync(u => u.Id == userId, ct);
    }

    public async Task<User?> GetByIdAsync(Guid userId, CancellationToken ct)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId, ct);
        
        return user;
    }

    public async Task<User> UpdateAsync(User user, CancellationToken ct)
    {
        var existingUser = GetByIdAsync(user.Id, ct).Result;
        
        existingUser!.UserName = user.UserName;
        existingUser.Email = user.Email;
        existingUser.Bio = user.Bio;
        existingUser.Location = user.Location;
        existingUser.ProfilePictureUrl = user.ProfilePictureUrl;
        existingUser.PasswordHash = user.PasswordHash;
        
        await _db.SaveChangesAsync(ct);
        return existingUser;
    }

    public async Task DeleteAsync(Guid userId, CancellationToken ct)
    {
        var user = GetByIdAsync(userId, ct).Result;
        _db.Users.Remove(user!);
        await _db.SaveChangesAsync(ct);
    }
}