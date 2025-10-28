using JamSpace.Domain.Entities;

namespace JamSpace.Application.Common.Interfaces;

public interface IUserFollowRepository
{
    Task<bool> UserFollowsAsync(Guid followerId, Guid followedId, CancellationToken ct);
    Task<HashSet<UserFollow>> GetFollowersAsync(Guid? userId, CancellationToken ct);
    Task<HashSet<UserFollow>> GetFollowingAsync(Guid? userId, CancellationToken ct);
    Task<UserFollow> FollowUserAsync(Guid followerId, Guid followedId, CancellationToken ct);
    Task UnfollowUserAsync(Guid followerId, Guid followedId, CancellationToken ct);
}