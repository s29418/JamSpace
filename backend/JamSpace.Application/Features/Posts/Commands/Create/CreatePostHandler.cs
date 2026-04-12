using JamSpace.Application.Common.Enums;
using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Common.Persistence;
using JamSpace.Application.Common.Validation;
using JamSpace.Application.Features.Posts.DTOs;
using JamSpace.Application.Features.Posts.Mappers;
using JamSpace.Domain.Entities;
using JamSpace.Domain.Enums;
using MediatR;

namespace JamSpace.Application.Features.Posts.Commands.Create;

public class CreatePostHandler : IRequestHandler<CreatePostCommand, PostDto>
{
    private readonly IPostRepository _post;
    private readonly IUserRepository _user;
    private readonly IUnitOfWork _uow;
    private readonly IFileStorageService _fileStorageService;

    public CreatePostHandler(IPostRepository post, IUserRepository user, 
        IUnitOfWork uow, IFileStorageService fileStorageService)
    {
        _post = post;
        _user = user;
        _uow = uow;
        _fileStorageService = fileStorageService;
    }

    public async Task<PostDto> Handle(CreatePostCommand c, CancellationToken ct)
    {
        var author = await _user.GetByIdAsync(c.AuthorId, ct)
                     ?? throw new NotFoundException("User not found");

        string? uploadedUrl = null;
        PostMedia? media = null;
        var postId = Guid.NewGuid();

        try
        {
            if (c.File is not null)
            {
                var category = MediaValidationRules.ResolveCategory(c.File.ContentType);

                if (category is null)
                    throw new InvalidOperationException("Unsupported media type.");
                
                var mediaType = category switch
                {
                    MediaCategory.Image => MediaType.Image,
                    MediaCategory.Audio => MediaType.Audio,
                    MediaCategory.Video => MediaType.Video,
                    _ => throw new InvalidOperationException("Unsupported media type.")
                };

                var storageType = category switch
                {
                    MediaCategory.Image => StorageObjectType.PostImage,
                    MediaCategory.Audio => StorageObjectType.PostAudio,
                    MediaCategory.Video => StorageObjectType.PostVideo,
                    _ => throw new InvalidOperationException("Unsupported media type.")
                };
                
                uploadedUrl = await _fileStorageService.UploadAsync(c.File, storageType, postId, ct);

                media = new PostMedia
                {
                    Id = Guid.NewGuid(),
                    PostId = postId,
                    Url = uploadedUrl,
                    MediaType = mediaType,
                    OriginalFileName = c.File.FileName,
                    ContentType = c.File.ContentType ?? "application/octet-stream",
                    Length = c.File.Length
                };
            }

            var post = new Post
            {
                Id = postId,
                AuthorId = c.AuthorId,
                Content = string.IsNullOrWhiteSpace(c.Content) ? null : c.Content.Trim(),
                CreatedAt = DateTimeOffset.UtcNow,
                Media = media
            };

            await _post.AddAsync(post, ct);
            await _uow.SaveChangesAsync(ct);

            post.Author = author;

            return PostMapper.ToDto(post, false, c.AuthorId);
        }
        catch 
        {
            if (!string.IsNullOrWhiteSpace(uploadedUrl))
            {
                try
                {
                    await _fileStorageService.DeleteAsync(uploadedUrl, ct);
                }
                catch
                {
                    // ignored
                }
            }

            throw;
        }
    }
}