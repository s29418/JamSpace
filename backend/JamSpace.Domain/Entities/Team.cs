namespace JamSpace.Domain.Entities;

public class Team
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public string? TeamPictureUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public Guid CreatedById { get; set; }
    public User CreatedBy { get; set; } = null!;
    
    public ICollection<TeamMember> Members { get; set; } = new List<TeamMember>();
    public ICollection<TeamEvent> Events { get; set; } = new List<TeamEvent>();
    public ICollection<Project> Projects { get; set; } = new List<Project>();
}