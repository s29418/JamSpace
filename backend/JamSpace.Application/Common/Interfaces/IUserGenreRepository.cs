using JamSpace.Domain.Entities;

namespace JamSpace.Application.Common.Interfaces;

public interface IUserGenreRepository
{
    Task<bool> UserHasGenreAsync(Guid userId, Guid genreId, CancellationToken ct);
    Task<List<UserGenre>> GetAllUserGenresAsync(Guid userId, CancellationToken ct);
    Task<UserGenre?> GetUserGenreAsync(Guid userId, Guid genreId, CancellationToken ct);
    Task AddAsync(UserGenre userGenre, CancellationToken ct);
    void Remove(UserGenre userGenre);
}