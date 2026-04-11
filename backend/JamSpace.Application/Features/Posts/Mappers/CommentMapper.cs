using JamSpace.Application.Features.Posts.DTOs;
using JamSpace.Domain.Entities;

namespace JamSpace.Application.Features.Posts.Mappers;

public static class CommentMapper
{
    public static PostCommentDto ToDto(PostComment comment)
    {
        return new PostCommentDto
        {
            Id = comment.Id,
            PostId = comment.PostId,
            UserId = comment.UserId,
            UserDisplayName = comment.User?.DisplayName ?? string.Empty,
            UserProfilePictureUrl = comment.User?.ProfilePictureUrl,
            Content = comment.Content,
            CreatedAt = comment.CreatedAt
        };
    }
}