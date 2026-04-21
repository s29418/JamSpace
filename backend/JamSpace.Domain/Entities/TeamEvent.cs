namespace JamSpace.Domain.Entities;

public class TeamEvent
{
    public Guid Id { get; set; }
    
    public Guid TeamId { get; set; }
    public Team Team { get; set; } = null!;
    
    public Guid CreatedById { get; set; }
    public User CreatedBy { get; set; } = null!;
    
    public required string Title { get; set; }
    public string? Description { get; set; }
    public DateTimeOffset StartDateTime { get; set; }
    public int DurationMinutes { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}