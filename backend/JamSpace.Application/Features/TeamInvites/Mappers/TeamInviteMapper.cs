using JamSpace.Application.Features.TeamInvites.DTOs;
using JamSpace.Domain.Entities;

namespace JamSpace.Application.Features.TeamInvites.Mappers;

public static class TeamInviteMapper
{
    public static TeamInviteDto ToDto(TeamInvite teamInvite)
    {
        return new TeamInviteDto
        {
            Id = teamInvite.Id,
            TeamId = teamInvite.TeamId,
            TeamName = teamInvite.Team.Name,
            TeamPictureUrl = teamInvite.Team.TeamPictureUrl,
            CreatedAt = teamInvite.CreatedAt,
            InvitedByUserName = teamInvite.InvitedByUser.UserName,
            InvitedUserName = teamInvite.InvitedUser.UserName,
            InvitedUserId = teamInvite.InvitedUser.Id,
            InvitedUserPictureUrl = teamInvite.InvitedUser.ProfilePictureUrl,
            Status = teamInvite.Status.ToString()
        };
    }
}
