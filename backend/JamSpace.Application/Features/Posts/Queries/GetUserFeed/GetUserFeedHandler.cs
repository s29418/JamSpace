using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Common.Models;
using JamSpace.Application.Features.Posts.DTOs;
using JamSpace.Application.Features.Posts.Mappers;
using MediatR;

namespace JamSpace.Application.Features.Posts.Queries.GetUserFeed;

public class GetUserFeedHandler : IRequestHandler<GetUserFeedQuery, CursorResult<PostDto>>
{
    private readonly IPostRepository _post;

    public GetUserFeedHandler(IPostRepository post)
    {
        _post = post;
    }

    public async Task<CursorResult<PostDto>> Handle(GetUserFeedQuery request, CancellationToken cancellationToken)
    {
        var take = Math.Clamp(request.Take, 1, 100);
        var takePlusOne = take + 1;

        var posts = await _post.GetPostsByAuthorAsync(request.AuthorId, request.Before, takePlusOne,
            cancellationToken);
        
        if (posts.Count == 0)
            return CursorResult<PostDto>.Create(Array.Empty<PostDto>(), false, null);

        var hasMore = posts.Count == takePlusOne;

        var pagePosts = hasMore ? posts.Take(take).ToList() : posts.ToList();

        var nextBefore = pagePosts.Last().CreatedAt;

        var dtoPosts = PostMapper.ToDto(pagePosts);
        
        return CursorResult<PostDto>.Create(dtoPosts, hasMore, nextBefore);
    }
}