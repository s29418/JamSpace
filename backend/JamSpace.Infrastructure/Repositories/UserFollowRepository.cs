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
        return await _db.UserFollows.AnyAsync(uf =>
            uf.FollowerId == followerId && uf.FolloweeId == followedId, ct);
    }


    public async Task<List<UserFollow>> GetFollowersAsync(Guid userId, CancellationToken ct)
    {
        return await _db.UserFollows
            .Where(uf => uf.FolloweeId == userId)
            .OrderByDescending(uf => uf.FollowedAt)
            .Include(uf => uf.Follower)
            .ToListAsync(ct);
    }

    public async Task<List<UserFollow>> GetFollowingAsync(Guid userId, CancellationToken ct)
    {
        return await _db.UserFollows
            .Where(uf => uf.FollowerId == userId)
            .OrderByDescending(uf => uf.FollowedAt)
            .Include(uf => uf.Followee)
            .ToListAsync(ct);
    }

    public async Task<UserFollow> FollowUserAsync(Guid followerId, Guid followedId, CancellationToken ct)
    {
        var userFollow = new UserFollow
        {
            FollowerId = followerId,
            FolloweeId = followedId,
            FollowedAt = DateTime.UtcNow
        };
        
        await _db.UserFollows.AddAsync(userFollow, ct);
        await _db.SaveChangesAsync(ct);
        
        return userFollow;
    }

    public async Task UnfollowUserAsync(Guid followerId, Guid followedId, CancellationToken ct)
    {
        var userFollow = await _db.UserFollows
            .FirstOrDefaultAsync(uf => uf.FollowerId == followerId && uf.FolloweeId == followedId, ct);
        
        if (userFollow != null)
        {
            _db.UserFollows.Remove(userFollow);
            await _db.SaveChangesAsync(ct);
        }
    }
}