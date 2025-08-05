using JamSpace.Application.Features.TeamInvites.DTOs;
using MediatR;

namespace JamSpace.Application.Features.TeamInvites.Commands.CancelTeamInvite;

public record CancelTeamInviteCommand(Guid TeamInviteId, Guid RequestingUserId) : IRequest<TeamInviteDto>;