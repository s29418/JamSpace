using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Common.Models;
using JamSpace.Application.Features.Posts.DTOs;
using JamSpace.Application.Features.Posts.Mappers;
using MediatR;

namespace JamSpace.Application.Features.Posts.Queries.GetFollowedFeed;

public class GetFollowedFeedHandler : IRequestHandler<GetFollowedFeedQuery, CursorResult<PostDto>>
{
    private readonly IPostRepository _post;

    public GetFollowedFeedHandler(IPostRepository post)
    {
        _post = post;
    }

    public async Task<CursorResult<PostDto>> Handle(GetFollowedFeedQuery request, CancellationToken cancellationToken)
    {
        var take = Math.Clamp(request.Take, 1, 50);
        var takePlusOne = take + 1;

        var posts = await _post.GetFollowedUsersPostsAsync(request.UserId, request.Before, takePlusOne,
            cancellationToken);
        
        if (posts.Count == 0)
            return CursorResult<PostDto>.Create(Array.Empty<PostDto>(), false, null);

        var hasMore = posts.Count == takePlusOne;

        var pagePosts = hasMore ? posts.Take(take).ToList() : posts.ToList();

        var nextBefore = pagePosts.Last().CreatedAt;

        var stats = await _post.GetPostStatsAsync(
            pagePosts
                .SelectMany(p => p.OriginalPost is null ? [p.Id] : new[] { p.Id, p.OriginalPost.Id }),
            request.UserId,
            cancellationToken);

        var dtoPosts = pagePosts
            .Select(p => PostMapper.ToDto(p, false, request.UserId, stats))
            .ToList();
        
        return CursorResult<PostDto>.Create(dtoPosts, hasMore, nextBefore);
    }
}
