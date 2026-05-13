namespace JamSpace.Application.Features.PortfolioTracks.DTOs;

public sealed record PortfolioTrackDto(
    Guid Id,
    Guid UserId,
    string Source,
    string Title,
    string? ArtistName,
    string? AlbumTitle,
    string? ArtworkUrl,
    int? DurationMs,
    string? ExternalTrackId,
    string? ExternalUrl,
    string? EmbedUrl,
    string? FileUrl,
    string? ContentType,
    long? Length,
    int DisplayOrder,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt
);
