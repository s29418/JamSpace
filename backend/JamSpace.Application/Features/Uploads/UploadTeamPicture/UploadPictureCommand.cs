using JamSpace.Application.Common.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace JamSpace.Application.Features.Uploads.UploadTeamPicture;

public record UploadPictureCommand(IFormFile File, PictureType Type, Guid? RelatedEntityId, Guid? RequestingUserId) : IRequest<string>;

