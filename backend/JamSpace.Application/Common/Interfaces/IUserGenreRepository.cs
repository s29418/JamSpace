using JamSpace.Domain.Entities;

namespace JamSpace.Application.Common.Interfaces;

public interface IUserGenreRepository
{
    Task<bool> UserHasGenreAsync(Guid userId, Guid skillId, CancellationToken ct);
    Task<List<UserGenre>> GetAllUserGenresAsync(Guid userId, CancellationToken ct);
    Task<UserGenre> AddUserGenreAsync(Guid userId, Guid skillId, CancellationToken ct);
    Task RemoveUserGenreAsync(Guid userId, Guid skillId, CancellationToken ct);
}