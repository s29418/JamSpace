using JamSpace.Domain.Entities;

namespace JamSpace.Application.Common.Interfaces;

public interface IUserFollowsRepository
{
    Task<bool> UserFollowsAsync(Guid followerId, Guid followedId, CancellationToken ct);
    Task<List<UserFollows>> GetFollowersAsync(Guid userId, CancellationToken ct);
    Task<List<UserFollows>> GetFollowingAsync(Guid userId, CancellationToken ct);
    Task<UserFollows> FollowUserAsync(Guid followerId, Guid followedId, CancellationToken ct);
    Task UnfollowUserAsync(Guid followerId, Guid followedId, CancellationToken ct);
}