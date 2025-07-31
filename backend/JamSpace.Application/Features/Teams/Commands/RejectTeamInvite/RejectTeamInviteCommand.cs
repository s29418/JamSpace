using MediatR;

namespace JamSpace.Application.Common.Features.Teams.Commands.RejectTeamInvite;

public record RejectTeamInviteCommand(Guid InviteId, Guid UserId) : IRequest<Unit>;
