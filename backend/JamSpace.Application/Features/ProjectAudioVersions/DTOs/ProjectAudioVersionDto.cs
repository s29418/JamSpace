namespace JamSpace.Application.Features.ProjectAudioVersions.DTOs;

public class ProjectAudioVersionDto
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public Guid CreatedById { get; set; }
    public string CreatedByDisplayName { get; set; } = null!;
    public string? CreatedByAvatarUrl { get; set; }
    public string Name { get; set; } = null!;
    public string FileUrl { get; set; } = null!;
    public string OriginalFileName { get; set; } = null!;
    public string ContentType { get; set; } = null!;
    public long Length { get; set; }
    public int? DurationSeconds { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
