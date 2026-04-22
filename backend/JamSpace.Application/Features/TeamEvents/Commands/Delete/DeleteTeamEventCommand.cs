using MediatR;

namespace JamSpace.Application.Features.TeamEvents.Commands.Delete;

public record DeleteTeamEventCommand(Guid EventId, Guid TeamId, Guid RequestingUserId) : IRequest<Unit>;