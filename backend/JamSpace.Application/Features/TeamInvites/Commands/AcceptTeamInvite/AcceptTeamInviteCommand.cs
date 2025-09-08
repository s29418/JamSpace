using JamSpace.Application.Features.TeamInvites.DTOs;
using MediatR;

namespace JamSpace.Application.Features.TeamInvites.Commands.AcceptTeamInvite;

public record AcceptTeamInviteCommand(Guid InviteId, Guid UserId) : IRequest<TeamInviteDto>;
