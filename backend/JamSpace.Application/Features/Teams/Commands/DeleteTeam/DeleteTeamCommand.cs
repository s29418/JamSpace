using MediatR;

namespace JamSpace.Application.Features.Teams.Commands.DeleteTeam;

public record DeleteTeamCommand(Guid TeamId, Guid RequestingUserId) : IRequest<Unit>;