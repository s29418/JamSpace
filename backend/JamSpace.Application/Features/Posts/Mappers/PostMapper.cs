using JamSpace.Application.Features.Posts.DTOs;
using JamSpace.Domain.Entities;

namespace JamSpace.Application.Features.Posts.Mappers;

public static class PostMapper
{
    public static PostDto ToDto(Post? post)
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
            AuthorAvatarUrl = post.Author?.ProfilePictureUrl
        };
    }
}