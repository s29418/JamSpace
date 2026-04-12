using JamSpace.Application.Features.Posts.DTOs;
using JamSpace.Domain.Entities;

namespace JamSpace.Application.Common.Interfaces;

public interface IPostRepository
{
    Task<Post?> GetByIdAsync(Guid id, CancellationToken ct);

    Task<IReadOnlyList<Post>> GetPostsByAuthorAsync(Guid authorId, DateTimeOffset? before,
        int take, CancellationToken ct);

    Task<IReadOnlyList<Post>> GetFollowedUsersPostsAsync(Guid userId, DateTimeOffset? before,
        int take, CancellationToken ct);

    Task<IReadOnlyList<Post>> GetExplorePostsAsync(DateTimeOffset? before, int take, CancellationToken ct);

    Task<Post?> GetRepostByAuthorAndOriginalAsync(Guid authorId, Guid originalPostId, CancellationToken ct);

    Task<IReadOnlyDictionary<Guid, PostStatsDto>> GetPostStatsAsync(
        IEnumerable<Guid> postIds,
        Guid? currentUserId,
        CancellationToken ct);

    Task AddAsync(Post post, CancellationToken ct);
    void Delete(Post post);
}
