using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Features.Posts.DTOs;
using JamSpace.Application.Features.Posts.Mappers;
using MediatR;

namespace JamSpace.Application.Features.Posts.Queries.GetPostById;

public class GetPostByIdHandler : IRequestHandler<GetPostByIdQuery, PostDto>
{
    private readonly IPostRepository _post;

    public GetPostByIdHandler(IPostRepository post)
    {
        _post = post;
    }

    public async Task<PostDto> Handle(GetPostByIdQuery request, CancellationToken cancellationToken)
    {
        var post = await _post.GetByIdAsync(request.Id, cancellationToken) ??
                   throw new NotFoundException("Post not found");

        var stats = await _post.GetPostStatsAsync(
            CollectPostIds(post),
            request.RequestingUserId,
            cancellationToken);

        return PostMapper.ToDto(post, true, request.RequestingUserId, stats);
    }

    private static IEnumerable<Guid> CollectPostIds(PostDto? dto) => Array.Empty<Guid>();

    private static IReadOnlyCollection<Guid> CollectPostIds(Domain.Entities.Post post)
    {
        var ids = new HashSet<Guid> { post.Id };

        if (post.OriginalPost is not null)
            ids.Add(post.OriginalPost.Id);

        return ids;
    }
}
