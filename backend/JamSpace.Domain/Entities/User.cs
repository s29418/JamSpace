namespace JamSpace.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public string? Bio { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public string? Location { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? SpotifyUrl { get; set; }
    public string? SoundcloudUrl { get; set; }
    
    public ICollection<TeamMember> Teams { get; set; } = new List<TeamMember>();
    public ICollection<Team> CreatedTeams { get; set; } = new List<Team>();
}