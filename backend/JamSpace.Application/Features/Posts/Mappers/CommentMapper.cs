using JamSpace.Application.Features.Posts.DTOs;
using JamSpace.Domain.Entities;

namespace JamSpace.Application.Features.Posts.Mappers;

public static class CommentMapper
{
    public static PostCommentDto ToDto(PostComment comment)
    {
        var userDisplayName = !string.IsNullOrWhiteSpace(comment.User?.DisplayName)
            ? comment.User.DisplayName
            : comment.User?.UserName ?? string.Empty;

        return new PostCommentDto
        {
            Id = comment.Id,
            PostId = comment.PostId,
            UserId = comment.UserId,
            UserDisplayName = userDisplayName,
            UserProfilePictureUrl = comment.User?.ProfilePictureUrl,
            Content = comment.Content,
            CreatedAt = comment.CreatedAt
        };
    }
}
