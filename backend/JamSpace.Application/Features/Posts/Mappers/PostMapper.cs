using JamSpace.Application.Features.Posts.DTOs;
using JamSpace.Domain.Entities;

namespace JamSpace.Application.Features.Posts.Mappers;

public static class PostMapper
{
    public static PostDto ToDto(Post post, bool passAllComments, Guid? currentUserId)
    {
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
            
            LikeCount = post.Likes.Count,
            CommentCount = post.Comments.Count,
            RepostCount = post.Reposts.Count,
            
            IsLikedByCurrentUser = currentUserId.HasValue && 
                                   post.Likes.Any(l => l.UserId == currentUserId.Value),
            
            IsRepostedByCurrentUser = currentUserId.HasValue && 
                                      post.Reposts.Any(r => r.AuthorId == currentUserId.Value),
            
            OriginalPost = post.OriginalPost is null ? null : ToNestedDto(post.OriginalPost, currentUserId),
            
            Comments = passAllComments ? 
                post.Comments
                    .OrderBy(c => c.CreatedAt)
                    .Select(CommentMapper.ToDto)
                    .ToList()
                : 
                post.Comments
                    .OrderBy(c => c.CreatedAt)
                    .Take(2)
                    .Select(CommentMapper.ToDto)
                    .ToList()
            };
    }
    
    private static PostDto ToNestedDto(Post post, Guid? currentUserId)
    {
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
            LikeCount = post.Likes.Count,
            CommentCount = post.Comments.Count,
            RepostCount = post.Reposts.Count,
            IsLikedByCurrentUser = currentUserId.HasValue &&
                                   post.Likes.Any(l => l.UserId == currentUserId.Value),
            IsRepostedByCurrentUser = currentUserId.HasValue &&
                                      post.Reposts.Any(r => r.AuthorId == currentUserId.Value),
            OriginalPost = null,
            Comments = []
        };
    }
}