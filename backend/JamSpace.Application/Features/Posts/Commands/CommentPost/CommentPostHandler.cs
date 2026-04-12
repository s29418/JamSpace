using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Common.Persistence;
using JamSpace.Domain.Entities;
using MediatR;

namespace JamSpace.Application.Features.Posts.Commands.CommentPost;

public class CommentPostHandler : IRequestHandler<CommentPostCommand, Unit>
{
    private readonly IPostCommentRepository _comment;
    private readonly IPostRepository _post;
    private readonly IUnitOfWork _uow;
    
    public CommentPostHandler(IPostCommentRepository comment, IPostRepository post, IUnitOfWork uow)
    {
        _comment = comment;
        _post = post;
        _uow = uow;
    }

    public async Task<Unit> Handle(CommentPostCommand request, CancellationToken cancellationToken)
    {
        var post = await _post.GetByIdAsync(request.PostId, cancellationToken);

        if (post is null)
            throw new NotFoundException("Post not found.");

        var comment = new PostComment
        {
            Id = Guid.NewGuid(),
            PostId = request.PostId,
            UserId = request.UserId,
            Content = request.Content.Trim(),
            CreatedAt = DateTimeOffset.UtcNow
        };

        await _comment.AddAsync(comment, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        
        return Unit.Value;
    }
}