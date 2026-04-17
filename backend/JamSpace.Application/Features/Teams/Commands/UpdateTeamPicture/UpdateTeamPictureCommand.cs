using JamSpace.Application.Common.Models;
using MediatR;

namespace JamSpace.Application.Features.Teams.Commands.UpdateTeamPicture;

public record UpdateTeamPictureCommand(
    Guid TeamId,
    Guid RequestingUserId,
    FileUpload File
) : IRequest<string>;