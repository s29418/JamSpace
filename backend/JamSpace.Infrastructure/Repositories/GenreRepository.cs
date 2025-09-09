using JamSpace.Application.Common.Interfaces;
using JamSpace.Domain.Entities;
using JamSpace.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace JamSpace.Infrastructure.Repositories;

public class GenreRepository : IGenreRepository
{
    private readonly JamSpaceDbContext _db;
    public GenreRepository(JamSpaceDbContext db)
    {
        _db = db;
    }
    
    public async Task<Genre> GetGenreByIdAsync(Guid genreId, CancellationToken ct)
    {
        var genre = await _db.Genres.FirstOrDefaultAsync(g => g.Id == genreId, ct) 
            ?? throw new KeyNotFoundException("Genre not found.");
        
        return genre;
    }

    public async Task<Genre> CreateGenreAsync(string genreName, CancellationToken ct)
    {
        var genre = new Genre
        {
            Id = Guid.NewGuid(),
            Name = genreName
        };
        
        _db.Genres.Add(genre);
        await _db.SaveChangesAsync(ct);
        return genre;
    }
}