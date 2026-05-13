namespace JamSpace.API.Requests;

public class UploadPortfolioTrackRequest
{
    public string Title { get; set; } = "";
    public string? ArtistName { get; set; }
    public string? AlbumTitle { get; set; }
    public int? DurationMs { get; set; }
    public IFormFile? File { get; set; }
}
