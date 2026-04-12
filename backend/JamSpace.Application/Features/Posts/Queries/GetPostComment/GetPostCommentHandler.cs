using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Features.Posts.DTOs;
using JamSpace.Application.Features.Posts.Mappers;
using MediatR;

namespace JamSpace.Application.Features.Posts.Queries.GetPostComment;

public class GetPostCommentHandler : IRequestHandler<GetPostCommentQuery, PostCommentDto>
{
    private readonly IPostCommentRepository _comment;

    public GetPostCommentHandler(IPostCommentRepository comment)
    {
        _comment = comment;
    }

    public async Task<PostCommentDto> Handle(GetPostCommentQuery request, CancellationToken cancellationToken)
    {
        var comment = await _comment.GetByIdAsync(request.CommentId, cancellationToken)
            ?? throw new NotFoundException("Comment not found.");

        return CommentMapper.ToDto(comment);
    }
}