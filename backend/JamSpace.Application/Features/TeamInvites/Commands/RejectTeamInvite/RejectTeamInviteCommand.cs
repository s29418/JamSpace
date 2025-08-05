using JamSpace.Application.Features.TeamInvites.DTOs;
using MediatR;

namespace JamSpace.Application.Features.TeamInvites.Commands.RejectTeamInvite;

public record RejectTeamInviteCommand(Guid InviteId, Guid UserId) : IRequest<TeamInviteDto>;
