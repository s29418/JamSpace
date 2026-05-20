using JamSpace.Application.Features.ProjectNotes.DTOs;
using JamSpace.Domain.Entities;

namespace JamSpace.Application.Features.ProjectNotes.Mappers;

public static class ProjectNoteMapper
{
    public static ProjectNoteDto ToDto(ProjectNote note, IReadOnlyDictionary<Guid, string?>? musicalRoles = null)
    {
        return new ProjectNoteDto
        {
            Id = note.Id,
            ProjectId = note.ProjectId,
            AudioVersionId = note.AudioVersionId,
            AudioVersionName = note.AudioVersion?.Name ?? note.AudioVersionNameSnapshot,
            IsAudioVersionDeleted = note.IsAudioVersionDeleted,
            CreatedById = note.CreatedById,
            CreatedByDisplayName = note.CreatedBy?.DisplayName ?? "Unknown user",
            CreatedByAvatarUrl = note.CreatedBy?.ProfilePictureUrl,
            CreatedByMusicalRole = GetMusicalRole(musicalRoles, note.CreatedById),
            CompletedById = note.CompletedById,
            CompletedByDisplayName = note.CompletedBy?.DisplayName,
            CompletedByAvatarUrl = note.CompletedBy?.ProfilePictureUrl,
            CompletedByMusicalRole = note.CompletedById.HasValue
                ? GetMusicalRole(musicalRoles, note.CompletedById.Value)
                : null,
            Content = note.Content,
            StartTimeSeconds = note.StartTimeSeconds,
            EndTimeSeconds = note.EndTimeSeconds,
            Status = note.Status.ToString(),
            CreatedAt = note.CreatedAt,
            UpdatedAt = note.UpdatedAt,
            CompletedAt = note.CompletedAt
        };
    }

    private static string? GetMusicalRole(IReadOnlyDictionary<Guid, string?>? musicalRoles, Guid userId)
    {
        return musicalRoles is not null && musicalRoles.TryGetValue(userId, out var role) ? role : null;
    }
}
