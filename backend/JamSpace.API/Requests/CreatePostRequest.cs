namespace JamSpace.API.Requests;

public class CreatePostRequest
{
    public string? Content { get; set; }
    public IFormFile? File { get; set; }
    public Guid? PortfolioTrackId { get; set; }
    public string? SpotifyPlaylistTitle { get; set; }
    public string? SpotifyPlaylistExternalUrl { get; set; }
}
