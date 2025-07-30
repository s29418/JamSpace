using MediatR;

namespace JamSpace.Application.Features.Teams.Commands.SendTeamInvite;

public record SendTeamInviteCommand(string InvitedUserName, Guid TeamId, Guid InvitingUserId) 
    : IRequest<Unit>;