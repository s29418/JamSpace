using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Common.Persistence;
using MediatR;

namespace JamSpace.Application.Features.Posts.Commands.Unlike;

public class UnlikePostHandler : IRequestHandler<UnlikePostCommand, Unit>
{
    private readonly IPostLikeRepository _like;
    private readonly IPostRepository _posts;
    private readonly IUnitOfWork _uow;

    public UnlikePostHandler(IPostLikeRepository like, IPostRepository posts, IUnitOfWork uow)
    {
        _like = like;
        _posts = posts;
        _uow = uow;
    }

    public async Task<Unit> Handle(UnlikePostCommand request, CancellationToken cancellationToken)
    {
        var post = await _posts.GetByIdAsync(request.PostId, cancellationToken);

        if (post is null)
            throw new NotFoundException("Post not found.");

        var exists = await _like.ExistsAsync(request.PostId, request.UserId, cancellationToken);

        if (!exists)
            throw new ConflictException("You have not liked this post.");

        await _like.DeleteByPostAndUserAsync(request.PostId, request.UserId, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}