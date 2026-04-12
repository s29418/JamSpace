using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Common.Persistence;
using JamSpace.Application.Features.Posts.DTOs;
using JamSpace.Application.Features.Posts.Mappers;
using JamSpace.Domain.Entities;
using MediatR;

namespace JamSpace.Application.Features.Posts.Commands.CommentPost;

public class CommentPostHandler : IRequestHandler<CommentPostCommand, PostCommentDto>
{
    private readonly IPostCommentRepository _comment;
    private readonly IPostRepository _post;
    private readonly IUserRepository _user;
    private readonly IUnitOfWork _uow;
    
    public CommentPostHandler(IPostCommentRepository comment, IPostRepository post, IUserRepository user, IUnitOfWork uow)
    {
        _comment = comment;
        _post = post;
        _user = user;
        _uow = uow;
    }

    public async Task<PostCommentDto> Handle(CommentPostCommand request, CancellationToken cancellationToken)
    {
        var post = await _post.GetByIdAsync(request.PostId, cancellationToken);

        if (post is null)
            throw new NotFoundException("Post not found.");

        var user = await _user.GetByIdAsync(request.UserId, cancellationToken)
                   ?? throw new NotFoundException("User not found.");

        var comment = new PostComment
        {
            Id = Guid.NewGuid(),
            PostId = request.PostId,
            UserId = request.UserId,
            User = user,
            Content = request.Content.Trim(),
            CreatedAt = DateTimeOffset.UtcNow
        };

        await _comment.AddAsync(comment, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        return CommentMapper.ToDto(comment);
    }
}
