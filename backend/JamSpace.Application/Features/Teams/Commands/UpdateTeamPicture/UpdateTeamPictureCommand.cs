using MediatR;

namespace JamSpace.Application.Features.Teams.Commands.UpdateTeamPicture;

public record UpdateTeamPictureCommand(
    Guid TeamId,
    Guid RequestingUserId,
    string PictureUrl
) : IRequest;