using JamSpace.Application.Common.Exceptions;
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
            .OrderBy(ug => ug.AddedAt)
            .Include(ug => ug.Genre)
            .ToListAsync(ct);
    }

    public async Task<UserGenre> AddUserGenreAsync(Guid userId, Guid genreId, CancellationToken ct)
    {
        var userGenre = new UserGenre
        {
            UserId = userId,
            GenreId = genreId,
            AddedAt = DateTime.UtcNow
        };
        
        _db.UserGenres.Add(userGenre);
        await _db.SaveChangesAsync(ct);
        return userGenre;
    }

    public async Task RemoveUserGenreAsync(Guid userId, Guid genreId, CancellationToken ct)
    {
        var userGenre = await _db.UserGenres
            .FirstOrDefaultAsync(ug => ug.UserId == userId && ug.GenreId == genreId, ct);

        if (userGenre == null)
            throw new NotFoundException("UserGenre not found.");
        
        _db.UserGenres.Remove(userGenre);
        await _db.SaveChangesAsync(ct);
    }
}