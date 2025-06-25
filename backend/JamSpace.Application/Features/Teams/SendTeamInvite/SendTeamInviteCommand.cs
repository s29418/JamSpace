using MediatR;

namespace JamSpace.Application.Features.Teams.SendTeamInvite;

public record SendTeamInviteCommand(string InvitedUserName, Guid TeamId, Guid InvitingUserId) 
    : IRequest;