using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Domain.Common;
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
            ?? throw new NotFoundException("Genre not found.");
        
        return genre;
    }
    
    public async Task<Genre?> GetGenreByNameAsync(string name, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(name))
            return null;

        var normalized = NameConventions.NormalizeForQuery(name);
        return await _db.Genres.FirstOrDefaultAsync(
            g => g.Name.Replace(" ", "").Replace("-", "").Replace("_", "").ToLower() == normalized, ct);
    }

    public async Task<Genre> CreateGenreAsync(string genreName, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(genreName))
            throw new ArgumentException("Genre name is required.", nameof(genreName));
        
        var pretty = NameConventions.PrettifyForDisplay(genreName);

        var genre = new Genre
        {
            Id = Guid.NewGuid(),
            Name = pretty
        };
        
        _db.Genres.Add(genre);
        await _db.SaveChangesAsync(ct);
        return genre;
    }
}