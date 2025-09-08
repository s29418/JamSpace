using JamSpace.Domain.Enums;

namespace JamSpace.Domain.Entities;

public class TeamMember
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public Guid TeamId { get; set; }
    public Team Team { get; set; } = null!;
    
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    
    public FunctionalRole Role { get; set; } = FunctionalRole.Member;
    public string? MusicalRole { get; set; }
}