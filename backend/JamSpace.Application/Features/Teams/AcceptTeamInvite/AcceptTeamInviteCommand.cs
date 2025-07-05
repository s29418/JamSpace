using MediatR;

namespace JamSpace.Application.Features.Teams.AcceptTeamInvite;

public record AcceptTeamInviteCommand(Guid InviteId, Guid UserId) : IRequest<Unit>;
