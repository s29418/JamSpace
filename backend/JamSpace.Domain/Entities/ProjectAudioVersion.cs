namespace JamSpace.Domain.Entities;

public class ProjectAudioVersion
{
    public Guid Id { get; set; }

    public Guid ProjectId { get; set; }
    public Project Project { get; set; } = null!;

    public Guid CreatedById { get; set; }
    public User CreatedBy { get; set; } = null!;

    public required string Name { get; set; }
    public required string FileUrl { get; set; }
    public required string OriginalFileName { get; set; }
    public required string ContentType { get; set; }
    public long Length { get; set; }
    public int? DurationSeconds { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
}
