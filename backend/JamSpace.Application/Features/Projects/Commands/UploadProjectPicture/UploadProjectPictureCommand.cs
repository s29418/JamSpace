using JamSpace.Application.Common.Models;
using MediatR;

namespace JamSpace.Application.Features.Projects.Commands.UploadProjectPicture;

public record UploadProjectPictureCommand(Guid ProjectId, Guid TeamId, Guid RequestingUserId, FileUpload File) 
    : IRequest<string>;