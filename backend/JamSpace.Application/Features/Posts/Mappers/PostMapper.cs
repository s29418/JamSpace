using JamSpace.Application.Features.Posts.DTOs;
using JamSpace.Domain.Entities;

namespace JamSpace.Application.Features.Posts.Mappers;

public static class PostMapper
{
    public static PostDto ToDto(
        Post post,
        bool passAllComments,
        Guid? currentUserId,
        IReadOnlyDictionary<Guid, PostStatsDto>? statsByPostId = null)
    {
        var stats = ResolveStats(post, currentUserId, statsByPostId);

        return new PostDto
        {
            Id = post.Id,
            Content = post.Content,
            CreatedAt = post.CreatedAt,
            MediaUrl = post.Media?.Url,
            MediaType = post.Media?.MediaType.ToString(),

            AuthorId = post.AuthorId,
            AuthorDisplayName = post.Author?.DisplayName,
            AuthorAvatarUrl = post.Author?.ProfilePictureUrl,

            LikeCount = stats.LikeCount,
            CommentCount = stats.CommentCount,
            RepostCount = stats.RepostCount,

            IsLikedByCurrentUser = stats.IsLikedByCurrentUser,
            IsRepostedByCurrentUser = stats.IsRepostedByCurrentUser,

            OriginalPost = post.OriginalPost is null ? null : ToNestedDto(post.OriginalPost, currentUserId, statsByPostId),

            Comments = passAllComments
                ? post.Comments
                    .OrderByDescending(c => c.CreatedAt)
                    .Select(CommentMapper.ToDto)
                    .ToList()
                : post.Comments
                    .OrderByDescending(c => c.CreatedAt)
                    .Take(2)
                    .Select(CommentMapper.ToDto)
                    .ToList()
        };
    }

    private static PostDto ToNestedDto(
        Post post,
        Guid? currentUserId,
        IReadOnlyDictionary<Guid, PostStatsDto>? statsByPostId)
    {
        var stats = ResolveStats(post, currentUserId, statsByPostId);

        return new PostDto
        {
            Id = post.Id,
            Content = post.Content,
            CreatedAt = post.CreatedAt,
            MediaUrl = post.Media?.Url,
            MediaType = post.Media?.MediaType.ToString(),
            AuthorId = post.AuthorId,
            AuthorDisplayName = post.Author?.DisplayName,
            AuthorAvatarUrl = post.Author?.ProfilePictureUrl,
            LikeCount = stats.LikeCount,
            CommentCount = stats.CommentCount,
            RepostCount = stats.RepostCount,
            IsLikedByCurrentUser = stats.IsLikedByCurrentUser,
            IsRepostedByCurrentUser = stats.IsRepostedByCurrentUser,
            OriginalPost = null,
            Comments = []
        };
    }

    private static PostStatsDto ResolveStats(
        Post post,
        Guid? currentUserId,
        IReadOnlyDictionary<Guid, PostStatsDto>? statsByPostId)
    {
        if (statsByPostId is not null && statsByPostId.TryGetValue(post.Id, out var stats))
            return stats;

        return new PostStatsDto
        {
            LikeCount = post.Likes.Count,
            CommentCount = post.Comments.Count,
            RepostCount = post.Reposts.Count,
            IsLikedByCurrentUser = currentUserId.HasValue &&
                                   post.Likes.Any(l => l.UserId == currentUserId.Value),
            IsRepostedByCurrentUser = currentUserId.HasValue &&
                                      post.Reposts.Any(r => r.AuthorId == currentUserId.Value),
        };
    }
}
