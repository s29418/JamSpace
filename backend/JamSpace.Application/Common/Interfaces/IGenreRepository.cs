using JamSpace.Domain.Entities;

namespace JamSpace.Application.Common.Interfaces;

public interface IGenreRepository
{
    Task<Genre> GetGenreByIdAsync(Guid genreId, CancellationToken ct);
    Task<Genre?> GetGenreByNameAsync(string genreName, CancellationToken ct);
    Task<Genre> CreateGenreAsync(string genreName, CancellationToken ct);
}