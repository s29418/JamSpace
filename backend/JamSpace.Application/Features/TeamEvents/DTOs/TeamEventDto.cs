namespace JamSpace.Application.Features.TeamEvents.DTOs;

public class TeamEventDto
{
    public Guid Id { get; set; }
    public Guid TeamId { get; set; }
    public Guid CreatedById { get; set; }
    public string CreatedByDisplayName { get; set; } = null!;
    public string? CreatedByAvatarUrl { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public DateTimeOffset StartDateTime { get; set; }
    public int DurationMinutes { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}