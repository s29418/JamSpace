using MediatR;

namespace JamSpace.Application.Features.Teams.Commands.CancelTeamInvite;

public record CancelTeamInviteCommand(Guid TeamInviteId, Guid RequestingUserId) : IRequest<Unit>;