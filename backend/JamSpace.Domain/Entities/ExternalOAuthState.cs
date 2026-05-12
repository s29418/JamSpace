using JamSpace.Domain.Enums;

namespace JamSpace.Domain.Entities;

public class ExternalOAuthState
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }
    public User? User { get; set; }

    public ExternalMusicProvider Provider { get; set; }
    public required string State { get; set; }
    public required string CodeVerifier { get; set; }
    public string? ReturnUrl { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset ExpiresAt { get; set; }
    public DateTimeOffset? ConsumedAt { get; set; }
}
