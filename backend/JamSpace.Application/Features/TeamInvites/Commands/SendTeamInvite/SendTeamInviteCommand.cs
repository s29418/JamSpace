using JamSpace.Application.Features.TeamInvites.DTOs;
using MediatR;

namespace JamSpace.Application.Features.TeamInvites.Commands.SendTeamInvite;

public record SendTeamInviteCommand(string InvitedUserName, Guid TeamId, Guid InvitingUserId) 
    : IRequest<TeamInviteDto>;