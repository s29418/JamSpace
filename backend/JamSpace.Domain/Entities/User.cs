using JamSpace.Domain.ValueObjects;

namespace JamSpace.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    
    public required string UserName { get; set; }
    public required string DisplayName { get; set; }
    public string? Bio { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public string Email { get; set; } = "";
    
    public string PasswordHash { get; set; } = "";
    public int TokenVersion { get; set; } = 0;

    public Location? Location { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? SpotifyUrl { get; set; }
    public string? SoundcloudUrl { get; set; }
    
    public bool IsDeleted { get; set; }
    
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    
    public ICollection<TeamMember> Teams { get; set; } = new List<TeamMember>();
    public ICollection<Team> CreatedTeams { get; set; } = new List<Team>();
    
    public ICollection<UserFollow> Followers { get; set; } = new List<UserFollow>();
    public ICollection<UserFollow> Following { get; set; } = new List<UserFollow>();
    
    public ICollection<UserGenre> UserGenres { get; set; } = new List<UserGenre>();
    public ICollection<UserSkill> UserSkills { get; set; } = new List<UserSkill>();
}