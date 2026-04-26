using JamSpace.Application.Features.Projects.DTOs;
using JamSpace.Domain.Entities;

namespace JamSpace.Application.Features.Projects.Mappers;

public static class ProjectMapper
{
    public static ProjectDto ToDto(Project project)
    {
        return new ProjectDto
        {
            Id = project.Id,
            TeamId = project.TeamId,
            Name = project.Name,
            PictureUrl = project.PictureUrl,
            CreatedAt = project.CreatedAt
        };
    }
}