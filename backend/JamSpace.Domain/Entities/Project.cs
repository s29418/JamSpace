namespace JamSpace.Domain.Entities;

public class Project
{
    public Guid Id { get; set; }
    
    public Guid TeamId { get; set; }
    public Team Team { get; set; } = null!;
    
    public string Name { get; set; } = null!;
    public string PictureUrl { get; set; } = null!;
    
    public DateTime CreatedAt { get; set; }
}