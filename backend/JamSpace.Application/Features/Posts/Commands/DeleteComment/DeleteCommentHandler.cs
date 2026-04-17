using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Common.Persistence;
using MediatR;

namespace JamSpace.Application.Features.Posts.Commands.DeleteComment;

public class DeleteCommentHandler : IRequestHandler<DeleteCommentCommand, Unit>
{
    private readonly IPostCommentRepository _comment;
    private readonly IUnitOfWork _uow;

    public DeleteCommentHandler(IPostCommentRepository comment, IUnitOfWork uow)
    {
        _comment = comment;
        _uow = uow;
    }

    public async Task<Unit> Handle(DeleteCommentCommand request, CancellationToken cancellationToken)
    {
        var comment = await _comment.GetByIdAsync(request.CommentId, cancellationToken);

        if (comment is null)
            throw new NotFoundException("Comment not found.");

        if (comment.UserId != request.RequestingUserId)
            throw new ForbiddenAccessException("You can delete only your own comments.");

        _comment.Delete(comment);
        await _uow.SaveChangesAsync(cancellationToken);
        
        return Unit.Value;
    }
}