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

    public async Task<Genre?> GetGenreByIdAsync(Guid genreId, CancellationToken ct)
    {
        return await _db.Genres.FirstOrDefaultAsync(g => g.Id == genreId, ct);
    }

    public async Task<Genre?> GetGenreByNameAsync(string name, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(name))
            return null;

        var normalized = NameConventions.NormalizeForQuery(name);

        return await _db.Genres.FirstOrDefaultAsync(
            g => g.Name.Replace(" ", "").Replace("-", "").Replace("_", "").ToLower() == normalized,
            ct);
    }

    public async Task AddAsync(Genre genre, CancellationToken ct)
    {
        await _db.Genres.AddAsync(genre, ct);
    }
}