namespace DefaultNamespace;

public class TeamMember
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public Guid TeamId { get; set; }
    public Team Team { get; set; } = null!;
    
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    
    public string Role { get; set; } = "Member";
}