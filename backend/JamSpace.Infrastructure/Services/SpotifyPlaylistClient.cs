using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Features.ExternalAccounts.DTOs;

namespace JamSpace.Infrastructure.Services;

public sealed class SpotifyPlaylistClient : ISpotifyPlaylistClient
{
    private const string CurrentUserPlaylistsEndpoint = "https://api.spotify.com/v1/me/playlists";
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly HttpClient _httpClient;

    public SpotifyPlaylistClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IReadOnlyList<SpotifyPlaylistDto>> GetCurrentUserPublicPlaylistsAsync(
        string accessToken,
        CancellationToken ct)
    {
        var url = $"{CurrentUserPlaylistsEndpoint}?limit=50";
        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        using var response = await _httpClient.SendAsync(request, ct);
        var body = await response.Content.ReadAsStringAsync(ct);

        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException($"Spotify playlists request failed with status {(int)response.StatusCode}: {body}");

        var playlists = JsonSerializer.Deserialize<SpotifyPlaylistsResponse>(body, JsonOptions)
                        ?? throw new InvalidOperationException("Spotify playlists response was empty.");

        return playlists.Items
            .Where(x => x.Public == true)
            .Where(x => !string.IsNullOrWhiteSpace(x.Id) && !string.IsNullOrWhiteSpace(x.Name))
            .Select(x =>
            {
                var externalUrl = x.ExternalUrls?.Spotify
                                  ?? $"https://open.spotify.com/playlist/{Uri.EscapeDataString(x.Id)}";

                var imageUrl = x.Images?
                    .OrderByDescending(image => image.Height ?? 0)
                    .Select(image => image.Url)
                    .FirstOrDefault(url => !string.IsNullOrWhiteSpace(url));

                return new SpotifyPlaylistDto(
                    x.Id,
                    x.Name,
                    externalUrl,
                    $"https://open.spotify.com/embed/playlist/{Uri.EscapeDataString(x.Id)}",
                    imageUrl);
            })
            .ToList();
    }

    private sealed record SpotifyPlaylistsResponse(
        [property: JsonPropertyName("items")] SpotifyPlaylistItemResponse[] Items);

    private sealed record SpotifyPlaylistItemResponse(
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("public")] bool? Public,
        [property: JsonPropertyName("external_urls")] SpotifyExternalUrlsResponse? ExternalUrls,
        [property: JsonPropertyName("images")] SpotifyImageResponse[]? Images);

    private sealed record SpotifyExternalUrlsResponse(
        [property: JsonPropertyName("spotify")] string? Spotify);

    private sealed record SpotifyImageResponse(
        [property: JsonPropertyName("url")] string? Url,
        [property: JsonPropertyName("height")] int? Height);
}
