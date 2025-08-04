using JamSpace.Application.Features.Teams.Dtos;
using MediatR;

namespace JamSpace.Application.Features.Teams.Commands.RejectTeamInvite;

public record RejectTeamInviteCommand(Guid InviteId, Guid UserId) : IRequest<TeamInviteDto>;
