using MediatR;

namespace JamSpace.Application.Features.TeamMembers.Commands.LeaveTeam;

public record LeaveTeamCommand(Guid TeamId, Guid UserId) : IRequest<Unit>;