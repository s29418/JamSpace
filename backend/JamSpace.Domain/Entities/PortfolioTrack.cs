using JamSpace.Domain.Enums;

namespace JamSpace.Domain.Entities;

public class PortfolioTrack
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }
    public User? User { get; set; }

    public PortfolioTrackSource Source { get; set; }
    public Guid? ExternalAccountId { get; set; }
    public UserExternalAccount? ExternalAccount { get; set; }

    public required string Title { get; set; }
    public string? ArtistName { get; set; }
    public string? AlbumTitle { get; set; }
    public string? ArtworkUrl { get; set; }
    public int? DurationMs { get; set; }

    public string? ExternalTrackId { get; set; }
    public string? ExternalUrl { get; set; }
    public string? EmbedUrl { get; set; }

    public string? FileUrl { get; set; }
    public string? OriginalFileName { get; set; }
    public string? ContentType { get; set; }
    public long? Length { get; set; }

    public int DisplayOrder { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
}
