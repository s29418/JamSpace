using MediatR;

namespace JamSpace.Application.Features.Teams.Commands.RejectTeamInvite;

public record RejectTeamInviteCommand(Guid InviteId, Guid UserId) : IRequest<Unit>;
