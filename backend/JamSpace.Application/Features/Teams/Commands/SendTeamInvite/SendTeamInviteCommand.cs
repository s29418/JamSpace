using MediatR;

namespace JamSpace.Application.Common.Features.Teams.Commands.SendTeamInvite;

public record SendTeamInviteCommand(string InvitedUserName, Guid TeamId, Guid InvitingUserId) 
    : IRequest<Unit>;