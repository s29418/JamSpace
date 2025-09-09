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
    
    public async Task<Genre?> GetGenreByNameAsync(string name, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(name))
            return null;

        var normalized = NormalizeForQuery(name);
        return await _db.Genres.FirstOrDefaultAsync(
            g => g.Name.Replace(" ", "").Replace("-", "").Replace("_", "").ToLower() == normalized, ct);
    }

    public async Task<Genre> CreateGenreAsync(string genreName, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(genreName))
            throw new ArgumentException("Genre name is required.", nameof(genreName));
        
        var pretty = PrettifyForDisplay(genreName);

        var genre = new Genre
        {
            Id = Guid.NewGuid(),
            Name = pretty
        };
        
        _db.Genres.Add(genre);
        await _db.SaveChangesAsync(ct);
        return genre;
    }


    private static string NormalizeForQuery(string input)
    {
        var trimmed = input.Trim();
        var compact = trimmed
            .Replace(" ", string.Empty)
            .Replace("-", string.Empty)
            .Replace("_", string.Empty);
        return compact.ToLowerInvariant();
    }


    private static string PrettifyForDisplay(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        var compactSpaces = string.Join(" ", input.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries));

        var words = compactSpaces.Split(' ');
        for (int i = 0; i < words.Length; i++)
        {
            var w = words[i];
            if (w.Length == 0) continue;

            var first = char.ToUpper(w[0]);
            var rest = w.Length > 1 ? w.Substring(1).ToLower() : string.Empty;
            words[i] = first + rest;
        }

        return string.Join(' ', words);
    }
}