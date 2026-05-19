namespace JamSpace.Application.Features.ProjectNotes.DTOs;

public class ProjectNoteDto
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public Guid? AudioVersionId { get; set; }
    public string? AudioVersionName { get; set; }
    public bool IsAudioVersionDeleted { get; set; }
    public Guid CreatedById { get; set; }
    public string CreatedByDisplayName { get; set; } = null!;
    public string? CreatedByAvatarUrl { get; set; }
    public Guid? CompletedById { get; set; }
    public string? CompletedByDisplayName { get; set; }
    public string? CompletedByAvatarUrl { get; set; }
    public string Content { get; set; } = null!;
    public int? StartTimeSeconds { get; set; }
    public int? EndTimeSeconds { get; set; }
    public string Status { get; set; } = null!;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public DateTimeOffset? CompletedAt { get; set; }
}
