using MediatR;

namespace JamSpace.Application.Features.Teams.Commands.AcceptTeamInvite;

public record AcceptTeamInviteCommand(Guid InviteId, Guid UserId) : IRequest<Unit>;
