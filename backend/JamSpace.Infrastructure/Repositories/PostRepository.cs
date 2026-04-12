using JamSpace.Application.Common.Interfaces;
using JamSpace.Domain.Entities;
using JamSpace.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace JamSpace.Infrastructure.Repositories;

public class PostRepository : IPostRepository
{
    private readonly JamSpaceDbContext _db;

    public PostRepository(JamSpaceDbContext db)
    {
        _db = db;
    }
    
    public async Task<Post?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await _db.Posts
            .Include(p => p.Media)
            .Include(p => p.Author)
            .FirstOrDefaultAsync(p => p.Id == id, ct);
    }

    public async Task<IReadOnlyList<Post>> GetPostsByAuthorAsync(Guid authorId, DateTimeOffset? before, 
        int take, CancellationToken ct)
    {
        var query = _db.Posts
            .Include(p => p.Media)
            .Include(p => p.Author)
            .Where(p => p.AuthorId == authorId);
            
        if (before is not null)
            query = query.Where(p => p.CreatedAt < before);
            
        return await query
            .OrderByDescending(p => p.CreatedAt)
            .Take(take)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<Post>> GetFollowedUsersPostsAsync(Guid userId, DateTimeOffset? before, 
        int take, CancellationToken ct)
    {
        IQueryable<Post> query = _db.Posts
            .Include(p => p.Media)
            .Include(p => p.Author)
            .Where(p => _db.UserFollows.Any(f => f.FollowerId == userId && f.FolloweeId == p.AuthorId));

         if (before is not null)
             query = query.Where(p => p.CreatedAt < before);

         
         return await query
             .OrderByDescending(p => p.CreatedAt)
             .ThenByDescending(p => p.Id)
             .Take(take)
             .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<Post>> GetExplorePostsAsync(DateTimeOffset? before, int take, CancellationToken ct)
    {
        IQueryable<Post> query = _db.Posts
            .Include(p => p.Media)
            .Include(p => p.Author);

        if (before is not null)
            query = query.Where(p => p.CreatedAt < before);
            
        return await query
            .OrderByDescending(p => p.CreatedAt)
            .ThenByDescending(p => p.Id)
            .Take(take)
            .ToListAsync(ct);
    }

    public async Task<Post?> GetRepostByAuthorAndOriginalAsync(Guid authorId, Guid originalPostId, CancellationToken ct)
    {
        return await _db.Posts
            .Include(p => p.Author)
            .Include(p => p.OriginalPost)
            .ThenInclude(op => op!.Author)
            .Include(p => p.OriginalPost)
            .ThenInclude(op => op!.Media)
            .Include(p => p.Likes)
            .Include(p => p.Comments)
            .Include(p => p.Reposts)
            .FirstOrDefaultAsync(
                p => p.AuthorId == authorId && p.OriginalPostId == originalPostId,
                ct);
    }

    public async Task AddAsync(Post post, CancellationToken ct) => await _db.Posts.AddAsync(post, ct);
    public void Delete(Post post) => _db.Posts.Remove(post);
}