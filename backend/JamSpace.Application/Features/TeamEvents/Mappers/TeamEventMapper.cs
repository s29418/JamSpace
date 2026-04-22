using JamSpace.Application.Features.TeamEvents.DTOs;
using JamSpace.Domain.Entities;

namespace JamSpace.Application.Features.TeamEvents.Mappers;

public static class TeamEventMapper
{
    public static TeamEventDto ToDto(TeamEvent teamEvent)
    {
        return new TeamEventDto
        {
            Id = teamEvent.Id,
            TeamId = teamEvent.TeamId,
            CreatedById = teamEvent.CreatedById,
            CreatedByDisplayName = teamEvent.CreatedBy.DisplayName,
            CreatedByAvatarUrl = teamEvent.CreatedBy.ProfilePictureUrl,
            Title = teamEvent.Title,
            Description = teamEvent.Description,
            StartDateTime = teamEvent.StartDateTime,
            DurationMinutes = teamEvent.DurationMinutes,
            CreatedAt = teamEvent.CreatedAt
        };
    }
}