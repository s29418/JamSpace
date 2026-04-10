using JamSpace.Application.Common.Models;
using JamSpace.Application.Features.Users.DTOs;
using MediatR;

namespace JamSpace.Application.Features.Users.Commands.UpdateUserProfilePicture;

public sealed record UpdateUserProfilePictureCommand(
    Guid UserId,
    Guid RequestingUserId,
    FileUpload File
) : IRequest<UserDto>;