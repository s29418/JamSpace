using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Common.Persistence;
using MediatR;

namespace JamSpace.Application.Features.Posts.Commands.DeleteRepost;

public class DeleteRepostHandler : IRequestHandler<DeleteRepostCommand, Unit>
{
    private readonly IPostRepository _post;
    private readonly IUnitOfWork _uow;

    public DeleteRepostHandler(IPostRepository post, IUnitOfWork uow)
    {
        _post = post;
        _uow = uow;
    }

    public async Task<Unit> Handle(DeleteRepostCommand request, CancellationToken cancellationToken)
    {
        var repost = await _post.GetRepostByAuthorAndOriginalAsync(
            request.UserId,
            request.OriginalPostId,
            cancellationToken);

        if (repost is null)
            throw new NotFoundException("Repost not found.");

        _post.Delete(repost);
        await _uow.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}