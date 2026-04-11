using JamSpace.Application.Common.Interfaces;
using JamSpace.Domain.Entities;
using JamSpace.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace JamSpace.Infrastructure.Repositories;

public class PostLikeRepository : IPostLikeRepository
{
    private readonly JamSpaceDbContext _db;

    public PostLikeRepository(JamSpaceDbContext db)
    {
        _db = db;
    }

    public async Task<bool> ExistsAsync(Guid postId, Guid userId, CancellationToken ct)
    {
        return await _db.PostLikes
            .AnyAsync(p => p.PostId == postId && p.UserId == userId, ct);
    }

    public async Task AddAsync(PostLike like, CancellationToken ct)
    {
        await _db.PostLikes.AddAsync(like, ct);
    }

    public async Task DeleteByPostAndUserAsync(Guid postId, Guid userId, CancellationToken ct)
    {
        await _db.PostLikes
            .Where(p => p.PostId == postId && p.UserId == userId)
            .ExecuteDeleteAsync(ct);
    }
}