using JamSpace.Application.Features.Posts.DTOs;
using JamSpace.Domain.Entities;

namespace JamSpace.Application.Features.Posts.Mappers;

public static class PostMapper
{
    public static List<PostDto> ToDto(IReadOnlyList<Post> posts)
    {
        var postsDto = new List<PostDto>();

        foreach (var post in posts)
        {
            postsDto.Add(new PostDto
            {
                Id = post.Id,
                Content = post.Content,
                CreatedAt = post.CreatedAt,
                MediaUrl = post.Media?.Url,
                AuthorId = post.AuthorId,
                AuthorDisplayName = post.Author?.DisplayName,
                AuthorAvatarUrl = post.Author?.ProfilePictureUrl
            });
        }
        return postsDto;
    }
}