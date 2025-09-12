using JamSpace.Application.Common.Interfaces;
using JamSpace.Domain.Entities;
using JamSpace.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace JamSpace.Infrastructure.Repositories;

public class UserFollowsRepository : IUserFollowsRepository
{
    private readonly JamSpaceDbContext _db;
    
    public UserFollowsRepository(JamSpaceDbContext db)
    {
        _db = db;
    }
    
    
    public async Task<bool> UserFollowsAsync(Guid followerId, Guid followedId, CancellationToken ct)
    {
        return await _db.UserFollows.AnyAsync(uf => uf.FollowerId == followerId && uf.FollowedId == followedId, ct);
    }

    public async Task<List<UserFollows>> GetFollowersAsync(Guid userId, CancellationToken ct)
    {
        return await _db.UserFollows
            .Where(uf => uf.FollowedId == userId)
            .Include(uf => uf.Follower)
            .ToListAsync(ct);
    }

    public async Task<List<UserFollows>> GetFollowingAsync(Guid userId, CancellationToken ct)
    {
        return await _db.UserFollows
            .Where(uf => uf.FollowerId == userId)
            .Include(uf => uf.Followed)
            .ToListAsync(ct);
    }

    public async Task<UserFollows> FollowUserAsync(Guid followerId, Guid followedId, CancellationToken ct)
    {
        var userFollow = new UserFollows
        {
            FollowerId = followerId,
            FollowedId = followedId,
            FollowedAt = DateTime.UtcNow
        };
        
        await _db.UserFollows.AddAsync(userFollow, ct);
        await _db.SaveChangesAsync(ct);
        
        return userFollow;
    }

    public async Task UnfollowUserAsync(Guid followerId, Guid followedId, CancellationToken ct)
    {
        var userFollow = await _db.UserFollows
            .FirstOrDefaultAsync(uf => uf.FollowerId == followerId && uf.FollowedId == followedId, ct);
        
        if (userFollow != null)
        {
            _db.UserFollows.Remove(userFollow);
            await _db.SaveChangesAsync(ct);
        }
    }
}