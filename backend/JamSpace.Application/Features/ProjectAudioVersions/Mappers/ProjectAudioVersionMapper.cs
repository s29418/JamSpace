using JamSpace.Application.Features.ProjectAudioVersions.DTOs;
using JamSpace.Domain.Entities;

namespace JamSpace.Application.Features.ProjectAudioVersions.Mappers;

public static class ProjectAudioVersionMapper
{
    public static ProjectAudioVersionDto ToDto(ProjectAudioVersion version)
    {
        return new ProjectAudioVersionDto
        {
            Id = version.Id,
            ProjectId = version.ProjectId,
            CreatedById = version.CreatedById,
            CreatedByDisplayName = version.CreatedBy.DisplayName,
            CreatedByAvatarUrl = version.CreatedBy.ProfilePictureUrl,
            Name = version.Name,
            FileUrl = version.FileUrl,
            OriginalFileName = version.OriginalFileName,
            ContentType = version.ContentType,
            Length = version.Length,
            DurationSeconds = version.DurationSeconds,
            CreatedAt = version.CreatedAt
        };
    }
}
