using JamSpace.Application.Features.ProjectNotes.DTOs;
using JamSpace.Domain.Entities;

namespace JamSpace.Application.Features.ProjectNotes.Mappers;

public static class ProjectNoteMapper
{
    public static ProjectNoteDto ToDto(ProjectNote note)
    {
        return new ProjectNoteDto
        {
            Id = note.Id,
            ProjectId = note.ProjectId,
            AudioVersionId = note.AudioVersionId,
            AudioVersionName = note.AudioVersion?.Name ?? note.AudioVersionNameSnapshot,
            IsAudioVersionDeleted = note.IsAudioVersionDeleted,
            CreatedById = note.CreatedById,
            CreatedByDisplayName = note.CreatedBy.DisplayName,
            CreatedByAvatarUrl = note.CreatedBy.ProfilePictureUrl,
            CompletedById = note.CompletedById,
            CompletedByDisplayName = note.CompletedBy?.DisplayName,
            CompletedByAvatarUrl = note.CompletedBy?.ProfilePictureUrl,
            Content = note.Content,
            StartTimeSeconds = note.StartTimeSeconds,
            EndTimeSeconds = note.EndTimeSeconds,
            Status = note.Status.ToString(),
            CreatedAt = note.CreatedAt,
            UpdatedAt = note.UpdatedAt,
            CompletedAt = note.CompletedAt
        };
    }
}
