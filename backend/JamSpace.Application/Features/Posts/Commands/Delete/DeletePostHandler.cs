using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Common.Persistence;
using MediatR;

namespace JamSpace.Application.Features.Posts.Commands.Delete;

public class DeletePostHandler : IRequestHandler<DeletePostCommand, Unit>
{
    private readonly IPostRepository _post;
    private readonly IUnitOfWork _uow;
    private readonly IFileStorageService _fileStorageService;

    public DeletePostHandler(IPostRepository post, IUnitOfWork uow, IFileStorageService fileStorageService)
    {
        _post = post;
        _uow = uow;
        _fileStorageService = fileStorageService;
    }

    public async Task<Unit> Handle(DeletePostCommand request, CancellationToken cancellationToken)
    {
        var post = await _post.GetByIdAsync(request.PostId, cancellationToken)
                   ?? throw new NotFoundException("Post not found.");

        if (post.AuthorId != request.RequestingUserId)
            throw new ForbiddenAccessException("You can delete only your own posts.");

        var mediaUrl = post.Media?.Url;

        _post.Delete(post);
        await _uow.SaveChangesAsync(cancellationToken);

        if (!string.IsNullOrWhiteSpace(mediaUrl))
        {
            try
            {
                await _fileStorageService.DeleteAsync(mediaUrl, cancellationToken);
            }
            catch
            {
                // ignored
            }
        }

        return Unit.Value;
    }
}