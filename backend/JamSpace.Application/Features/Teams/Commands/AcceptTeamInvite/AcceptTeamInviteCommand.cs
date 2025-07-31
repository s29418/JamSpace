using MediatR;

namespace JamSpace.Application.Common.Features.Teams.Commands.AcceptTeamInvite;

public record AcceptTeamInviteCommand(Guid InviteId, Guid UserId) : IRequest<Unit>;
