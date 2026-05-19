namespace JamSpace.Application.Features.Posts.DTOs;

public sealed class PostSpotifyPlaylistDto
{
    public string Title { get; set; } = null!;
    public string ExternalUrl { get; set; } = null!;
    public string EmbedUrl { get; set; } = null!;
}
