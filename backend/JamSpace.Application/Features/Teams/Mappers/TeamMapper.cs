using JamSpace.Application.Features.Teams.Dtos;
using JamSpace.Domain.Entities;

namespace JamSpace.Application.Features.Teams.Mappers;

public static class TeamMapper
{
    public static TeamDto ToDto(Team team)
    {
        return new TeamDto
        {
            Id = team.Id,
            Name = team.Name,
            TeamPictureUrl = team.TeamPictureUrl,
            CreatedAt = team.CreatedAt,
            CreatedByUserName = team.CreatedBy.UserName,
            Members = team.Members
                .Select(m => new TeamMemberDto
            {
                UserId = m.User.Id,
                Username = m.User.UserName,
                Role = m.Role.ToString(),
                MusicalRole = m.MusicalRole?.ToString(),
                UserPictureUrl = m.User.ProfilePictureUrl
            }).ToList()
        };
    }
}
