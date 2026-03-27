using JamSpace.Application.Common.Interfaces;
using JamSpace.Domain.Entities;
using JamSpace.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace JamSpace.Infrastructure.Repositories;

public class UserFollowRepository : IUserFollowRepository
{
    private readonly JamSpaceDbContext _db;

    public UserFollowRepository(JamSpaceDbContext db)
    {
        _db = db;
    }

    public async Task<bool> UserFollowsAsync(Guid followerId, Guid followedId, CancellationToken ct)
    {
        return await _db.UserFollows.AnyAsync(
            uf => uf.FollowerId == followerId && uf.FolloweeId == followedId,
            ct);
    }

    public async Task<HashSet<UserFollow>> GetFollowersAsync(Guid userId, CancellationToken ct)
    {
        return await _db.UserFollows
            .Where(uf => uf.FolloweeId == userId)
            .OrderByDescending(uf => uf.FollowedAt)
            .Include(uf => uf.Follower)
            .ToHashSetAsync(ct);
    }

    public async Task<HashSet<UserFollow>> GetFollowingAsync(Guid userId, CancellationToken ct)
    {
        return await _db.UserFollows
            .Where(uf => uf.FollowerId == userId)
            .OrderByDescending(uf => uf.FollowedAt)
            .Include(uf => uf.Followee)
            .ToHashSetAsync(ct);
    }

    public async Task<UserFollow?> GetAsync(Guid followerId, Guid followedId, CancellationToken ct)
    {
        return await _db.UserFollows.FirstOrDefaultAsync(
            uf => uf.FollowerId == followerId && uf.FolloweeId == followedId,
            ct);
    }

    public async Task AddAsync(UserFollow userFollow, CancellationToken ct)
    {
        await _db.UserFollows.AddAsync(userFollow, ct);
    }

    public void Remove(UserFollow userFollow)
    {
        _db.UserFollows.Remove(userFollow);
    }
}