using JamSpace.Domain.Entities;

namespace JamSpace.Application.Common.Interfaces;

public interface IPostLikeRepository
{
    Task<bool> ExistsAsync(Guid postId, Guid userId, CancellationToken ct);
    Task AddAsync(PostLike like, CancellationToken ct);
    Task DeleteByPostAndUserAsync(Guid postId, Guid userId, CancellationToken ct);
}