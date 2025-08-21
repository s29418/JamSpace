using JamSpace.Application.Common.Enums;
using JamSpace.Application.Common.Models;
using MediatR;

namespace JamSpace.Application.Features.Uploads.UploadPicture;

public record UploadPictureCommand(
    FileUpload File,
    PictureType Type,
    Guid? RelatedEntityId,
    Guid? RequestingUserId
) : IRequest<string>;
