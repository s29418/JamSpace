using MediatR;

namespace JamSpace.Application.Features.Teams.SendTeamInvite;

public record SendTeamInviteCommand(Guid InvitedUserId, Guid TeamId, Guid InvitingUserId) 
    : IRequest;