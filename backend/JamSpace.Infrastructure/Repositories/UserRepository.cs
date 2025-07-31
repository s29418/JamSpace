using DefaultNamespace;
using JamSpace.Application.Interfaces;
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
}