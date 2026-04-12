using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Features.Posts.DTOs;
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
        return await BuildPostQuery()
            .FirstOrDefaultAsync(p => p.Id == id, ct);
    }

    public async Task<IReadOnlyList<Post>> GetPostsByAuthorAsync(
        Guid authorId,
        DateTimeOffset? before,
        int take,
        CancellationToken ct)
    {
        var query = BuildPostQuery()
            .Where(p => p.AuthorId == authorId);

        if (before is not null)
            query = query.Where(p => p.CreatedAt < before);

        return await query
            .OrderByDescending(p => p.CreatedAt)
            .Take(take)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<Post>> GetFollowedUsersPostsAsync(
        Guid userId,
        DateTimeOffset? before,
        int take,
        CancellationToken ct)
    {
        IQueryable<Post> query = BuildPostQuery()
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
        IQueryable<Post> query = BuildPostQuery();

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
        return await BuildPostQuery()
            .FirstOrDefaultAsync(
                p => p.AuthorId == authorId && p.OriginalPostId == originalPostId,
                ct);
    }

    public async Task<IReadOnlyDictionary<Guid, PostStatsDto>> GetPostStatsAsync(
        IEnumerable<Guid> postIds,
        Guid? currentUserId,
        CancellationToken ct)
    {
        var ids = postIds
            .Distinct()
            .ToArray();

        if (ids.Length == 0)
            return new Dictionary<Guid, PostStatsDto>();

        var likeCounts = await _db.PostLikes
            .Where(l => ids.Contains(l.PostId))
            .GroupBy(l => l.PostId)
            .Select(g => new { PostId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.PostId, x => x.Count, ct);

        var commentCounts = await _db.PostComments
            .Where(c => ids.Contains(c.PostId))
            .GroupBy(c => c.PostId)
            .Select(g => new { PostId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.PostId, x => x.Count, ct);

        var repostCounts = await _db.Posts
            .Where(p => p.OriginalPostId.HasValue && ids.Contains(p.OriginalPostId.Value))
            .GroupBy(p => p.OriginalPostId!.Value)
            .Select(g => new { PostId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.PostId, x => x.Count, ct);

        HashSet<Guid> likedByCurrentUser = [];
        HashSet<Guid> repostedByCurrentUser = [];

        if (currentUserId.HasValue)
        {
            likedByCurrentUser = await _db.PostLikes
                .Where(l => l.UserId == currentUserId.Value && ids.Contains(l.PostId))
                .Select(l => l.PostId)
                .ToHashSetAsync(ct);

            repostedByCurrentUser = await _db.Posts
                .Where(p => p.AuthorId == currentUserId.Value && p.OriginalPostId.HasValue && ids.Contains(p.OriginalPostId.Value))
                .Select(p => p.OriginalPostId!.Value)
                .ToHashSetAsync(ct);
        }

        return ids.ToDictionary(
            id => id,
            id => new PostStatsDto
            {
                LikeCount = likeCounts.GetValueOrDefault(id),
                CommentCount = commentCounts.GetValueOrDefault(id),
                RepostCount = repostCounts.GetValueOrDefault(id),
                IsLikedByCurrentUser = likedByCurrentUser.Contains(id),
                IsRepostedByCurrentUser = repostedByCurrentUser.Contains(id),
            });
    }

    public async Task AddAsync(Post post, CancellationToken ct) => await _db.Posts.AddAsync(post, ct);

    public void Delete(Post post) => _db.Posts.Remove(post);

    private IQueryable<Post> BuildPostQuery()
    {
        return _db.Posts
            .AsSplitQuery()
            .Include(p => p.Media)
            .Include(p => p.Author)
            .Include(p => p.Likes)
            .Include(p => p.Reposts)
            .Include(p => p.Comments)
            .Include(p => p.OriginalPost)
            .ThenInclude(op => op!.Author)
            .Include(p => p.OriginalPost)
            .ThenInclude(op => op!.Media)
            .Include(p => p.OriginalPost)
            .ThenInclude(op => op!.Likes)
            .Include(p => p.OriginalPost)
            .ThenInclude(op => op!.Reposts)
            .Include(p => p.OriginalPost)
            .ThenInclude(op => op!.Comments);
    }
}
