using MediatR;

namespace JamSpace.Application.Features.Teams.Commands.LeaveTeam;

public record LeaveTeamCommand(Guid TeamId, Guid UserId) : IRequest<Unit>;