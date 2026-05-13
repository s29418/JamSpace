using JamSpace.Domain.Enums;

namespace JamSpace.API.Requests;

public class AddExternalPortfolioTrackRequest
{
    public PortfolioTrackSource Source { get; set; }
    public string Title { get; set; } = "";
    public string? ArtistName { get; set; }
    public string? AlbumTitle { get; set; }
    public string? ArtworkUrl { get; set; }
    public int? DurationMs { get; set; }
    public string? ExternalTrackId { get; set; }
    public string ExternalUrl { get; set; } = "";
    public string? EmbedUrl { get; set; }
}
