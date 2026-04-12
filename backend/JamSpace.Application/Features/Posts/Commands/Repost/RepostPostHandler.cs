using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Common.Persistence;
using JamSpace.Application.Features.Posts.DTOs;
using JamSpace.Application.Features.Posts.Mappers;
using JamSpace.Domain.Entities;
using MediatR;

namespace JamSpace.Application.Features.Posts.Commands.Repost;

public class RepostPostHandler : IRequestHandler<RepostPostCommand, PostDto>
{
    private readonly IPostRepository _post;
    private readonly IUserRepository _user;
    private readonly IUnitOfWork _uow;

    public RepostPostHandler(IPostRepository post, IUserRepository user, IUnitOfWork uow)
    {
        _post = post;
        _user = user;
        _uow = uow;
    }

    public async Task<PostDto> Handle(RepostPostCommand request, CancellationToken cancellationToken)
    {
        var user = await _user.GetByIdAsync(request.UserId, cancellationToken)
                   ?? throw new NotFoundException("User not found.");

        var originalPost = await _post.GetByIdAsync(request.OriginalPostId, cancellationToken)
                           ?? throw new NotFoundException("Original post not found.");

        if (originalPost.OriginalPostId is not null)
            throw new ConflictException("Reposting a repost is not allowed.");

        var existingRepost = await _post.GetRepostByAuthorAndOriginalAsync(
            request.UserId,
            request.OriginalPostId,
            cancellationToken);

        if (existingRepost is not null)
            throw new ConflictException("You have already reposted this post.");

        var repost = new Post
        {
            Id = Guid.NewGuid(),
            AuthorId = request.UserId,
            Content = null,
            CreatedAt = DateTimeOffset.UtcNow,
            OriginalPostId = request.OriginalPostId
        };

        await _post.AddAsync(repost, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        repost.Author = user;
        repost.OriginalPost = originalPost;
        originalPost.Reposts.Add(repost);

        var stats = await _post.GetPostStatsAsync(
            new[] { repost.Id, originalPost.Id },
            request.UserId,
            cancellationToken);

        return PostMapper.ToDto(repost, false, request.UserId, stats);
    }
}
