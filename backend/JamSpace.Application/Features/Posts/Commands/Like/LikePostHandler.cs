using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Common.Persistence;
using JamSpace.Domain.Entities;
using MediatR;

namespace JamSpace.Application.Features.Posts.Commands.Like;

public class LikePostHandler : IRequestHandler<LikePostCommand, Unit>
{
    private readonly IPostLikeRepository _like;
    private readonly IPostRepository _post;
    private readonly IUnitOfWork _uow;

    public LikePostHandler(IPostLikeRepository like, IUnitOfWork uow, IPostRepository post)
    {
        _like = like;
        _uow = uow;
        _post = post;
    }

    public async Task<Unit> Handle(LikePostCommand request, CancellationToken cancellationToken)
    {
        var post = await _post.GetByIdAsync(request.PostId, cancellationToken);

        if (post is null)
            throw new NotFoundException("Post not found.");
        
        var exists = await _like.ExistsAsync(request.PostId, request.UserId, cancellationToken);

        if (exists)
            throw new ConflictException("You already liked this post.");
        
        var postLike = new PostLike
        {
            Id = Guid.NewGuid(),
            PostId = request.PostId,
            UserId = request.UserId,
            CreatedAt = DateTimeOffset.UtcNow
        };

        await _like.AddAsync(postLike, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        
        return Unit.Value;
    }
}