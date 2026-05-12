using JamSpace.Domain.Enums;

namespace JamSpace.Domain.Entities;

public class UserExternalAccount
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }
    public User? User { get; set; }

    public ExternalMusicProvider Provider { get; set; }
    public required string ExternalUserId { get; set; }
    public required string DisplayName { get; set; }
    public required string ProfileUrl { get; set; }
    public string? AvatarUrl { get; set; }

    public required string AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    public DateTimeOffset? TokenExpiresAt { get; set; }
    public string? Scopes { get; set; }

    public DateTimeOffset ConnectedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public DateTimeOffset? DisconnectedAt { get; set; }
}
