using MediatR;

namespace JamSpace.Application.Features.Teams.RejectTeamInvite;

public record RejectTeamInviteCommand(Guid InviteId, Guid UserId) : IRequest<Unit>;
