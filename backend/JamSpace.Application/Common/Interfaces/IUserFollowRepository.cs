using JamSpace.Domain.Entities;

namespace JamSpace.Application.Common.Interfaces;

public interface IUserFollowRepository
{
    Task<bool> UserFollowsAsync(Guid followerId, Guid followedId, CancellationToken ct);

    Task<HashSet<UserFollow>> GetFollowersAsync(Guid userId, CancellationToken ct);
    Task<HashSet<UserFollow>> GetFollowingAsync(Guid userId, CancellationToken ct);

    Task<UserFollow?> GetAsync(Guid followerId, Guid followedId, CancellationToken ct);

    Task AddAsync(UserFollow userFollow, CancellationToken ct);
    void Remove(UserFollow userFollow);
}