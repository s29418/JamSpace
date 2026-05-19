namespace JamSpace.Application.Features.Posts.Commands.Create;

internal static class SpotifyPlaylistLink
{
    public static bool TryBuildEmbedUrl(string? externalUrl, out string? embedUrl)
    {
        embedUrl = null;

        if (string.IsNullOrWhiteSpace(externalUrl))
            return false;

        if (!Uri.TryCreate(externalUrl.Trim(), UriKind.Absolute, out var uri))
            return false;

        var host = uri.Host.StartsWith("www.", StringComparison.OrdinalIgnoreCase)
            ? uri.Host[4..]
            : uri.Host;

        if (!string.Equals(host, "open.spotify.com", StringComparison.OrdinalIgnoreCase))
            return false;

        var parts = uri.AbsolutePath
            .Split('/', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        if (parts.Length < 2 || !string.Equals(parts[0], "playlist", StringComparison.OrdinalIgnoreCase))
            return false;

        var playlistId = parts[1];

        if (string.IsNullOrWhiteSpace(playlistId))
            return false;

        embedUrl = $"https://open.spotify.com/embed/playlist/{playlistId}";
        return true;
    }
}
