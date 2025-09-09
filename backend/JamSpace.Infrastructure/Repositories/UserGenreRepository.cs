using JamSpace.Application.Common.Interfaces;
using JamSpace.Domain.Entities;
using JamSpace.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace JamSpace.Infrastructure.Repositories;

public class UserGenreRepository : IUserGenreRepository
{
    private readonly JamSpaceDbContext _db;
    
    public UserGenreRepository(JamSpaceDbContext db)
    {
        _db = db;
    }
    
    
    public async Task<bool> UserHasGenreAsync(Guid userId, Guid genreId, CancellationToken ct)
    {
        return await _db.UserGenres.AnyAsync(ug => ug.UserId == userId && ug.GenreId == genreId, ct);
    }

    public async Task<List<UserGenre>> GetAllUserGenresAsync(Guid userId, CancellationToken ct)
    {
        return await _db.UserGenres
            .Where(ug => ug.UserId == userId)
            .ToListAsync(ct);
    }

    public async Task<UserGenre> AddUserGenreAsync(Guid userId, Guid genreId, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public async Task RemoveUserGenreAsync(Guid userId, Guid genreId, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}