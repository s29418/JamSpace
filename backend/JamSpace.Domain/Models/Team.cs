namespace DefaultNamespace;

public class Team
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? TeamPictureUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public Guid CreatedById { get; set; }
    public User CreatedBy { get; set; }
    
    public ICollection<TeamMember> Members { get; set; } = new List<TeamMember>();
}