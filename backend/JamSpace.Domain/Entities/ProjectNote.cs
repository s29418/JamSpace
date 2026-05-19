using JamSpace.Domain.Enums;

namespace JamSpace.Domain.Entities;

public class ProjectNote
{
    public Guid Id { get; set; }

    public Guid ProjectId { get; set; }
    public Project Project { get; set; } = null!;

    public Guid? AudioVersionId { get; set; }
    public ProjectAudioVersion? AudioVersion { get; set; }
    public string? AudioVersionNameSnapshot { get; set; }
    public bool IsAudioVersionDeleted { get; set; }

    public Guid CreatedById { get; set; }
    public User CreatedBy { get; set; } = null!;

    public Guid? CompletedById { get; set; }
    public User? CompletedBy { get; set; }

    public required string Content { get; set; }
    public int? StartTimeSeconds { get; set; }
    public int? EndTimeSeconds { get; set; }
    public ProjectNoteStatus Status { get; set; } = ProjectNoteStatus.Active;

    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public DateTimeOffset? CompletedAt { get; set; }
}
