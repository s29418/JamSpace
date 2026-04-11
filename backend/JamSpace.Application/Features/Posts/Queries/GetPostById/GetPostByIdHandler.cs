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

        return PostMapper.ToDto(post);
    }
}