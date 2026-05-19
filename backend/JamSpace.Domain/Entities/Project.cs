namespace JamSpace.Domain.Entities;

public class Project
{
    public Guid Id { get; set; }
    
    public Guid TeamId { get; set; }
    public Team Team { get; set; } = null!;
    
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string? PictureUrl { get; set; }
    
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    
    public ICollection<ProjectAudioVersion> AudioVersions { get; set; } = new List<ProjectAudioVersion>();
}
