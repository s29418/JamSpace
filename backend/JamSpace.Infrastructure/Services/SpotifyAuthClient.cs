using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Common.Models;
using JamSpace.Application.Common.Settings;
using JamSpace.Domain.Enums;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;

namespace JamSpace.Infrastructure.Services;

public class SpotifyAuthClient : IMusicPlatformAuthClient
{
    private const string DefaultAuthorizationEndpoint = "https://accounts.spotify.com/authorize";
    private const string DefaultTokenEndpoint = "https://accounts.spotify.com/api/token";
    private const string DefaultCurrentUserEndpoint = "https://api.spotify.com/v1/me";

    private static readonly string[] DefaultScopes = [
        "user-read-private",
        "user-read-email",
        "playlist-read-private"
    ];
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly HttpClient _httpClient;
    private readonly OAuthProviderSettings _settings;

    public SpotifyAuthClient(HttpClient httpClient, IOptions<MusicPlatformOAuthSettings> settings)
    {
        _httpClient = httpClient;
        _settings = settings.Value.Spotify;
    }

    public ExternalMusicProvider Provider => ExternalMusicProvider.Spotify;

    public ExternalAuthUrl BuildAuthorizationUrl(string state, string codeChallenge)
    {
        EnsureRequiredAuthSettings();

        var query = new Dictionary<string, string?>
        {
            ["response_type"] = "code",
            ["client_id"] = _settings.ClientId,
            ["redirect_uri"] = _settings.RedirectUri,
            ["state"] = state,
            ["scope"] = string.Join(' ', ResolveScopes()),
            ["code_challenge"] = codeChallenge,
            ["code_challenge_method"] = "S256"
        };

        return new ExternalAuthUrl(QueryHelpers.AddQueryString(ResolveAuthorizationEndpoint(), query));
    }

    public async Task<ExternalTokenResponse> ExchangeCodeAsync(
        string code,
        string codeVerifier,
        CancellationToken ct)
    {
        EnsureRequiredTokenSettings();

        using var request = new HttpRequestMessage(HttpMethod.Post, ResolveTokenEndpoint())
        {
            Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["grant_type"] = "authorization_code",
                ["code"] = code,
                ["redirect_uri"] = _settings.RedirectUri,
                ["client_id"] = _settings.ClientId,
                ["code_verifier"] = codeVerifier
            })
        };

        using var response = await _httpClient.SendAsync(request, ct);
        var body = await response.Content.ReadAsStringAsync(ct);

        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException($"Spotify token exchange failed with status {(int)response.StatusCode}: {body}");

        var token = JsonSerializer.Deserialize<SpotifyTokenResponse>(body, JsonOptions)
                    ?? throw new InvalidOperationException("Spotify token response was empty.");

        if (string.IsNullOrWhiteSpace(token.AccessToken))
            throw new InvalidOperationException("Spotify token response did not include an access token.");

        var expiresAt = token.ExpiresIn.HasValue
            ? DateTimeOffset.UtcNow.AddSeconds(token.ExpiresIn.Value)
            : (DateTimeOffset?)null;

        return new ExternalTokenResponse(
            token.AccessToken,
            token.RefreshToken,
            expiresAt,
            token.Scope);
    }

    public async Task<ExternalUserProfile> GetCurrentUserProfileAsync(
        string accessToken,
        CancellationToken ct)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, ResolveCurrentUserEndpoint());
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        using var response = await _httpClient.SendAsync(request, ct);
        var body = await response.Content.ReadAsStringAsync(ct);

        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException($"Spotify profile request failed with status {(int)response.StatusCode}: {body}");

        var profile = JsonSerializer.Deserialize<SpotifyUserProfileResponse>(body, JsonOptions)
                      ?? throw new InvalidOperationException("Spotify profile response was empty.");

        if (string.IsNullOrWhiteSpace(profile.Id))
            throw new InvalidOperationException("Spotify profile response did not include a user id.");

        var profileUrl = profile.ExternalUrls?.Spotify
                         ?? $"https://open.spotify.com/user/{Uri.EscapeDataString(profile.Id)}";

        var avatarUrl = profile.Images?
            .OrderByDescending(x => x.Height ?? 0)
            .Select(x => x.Url)
            .FirstOrDefault(x => !string.IsNullOrWhiteSpace(x));

        return new ExternalUserProfile(
            profile.Id,
            string.IsNullOrWhiteSpace(profile.DisplayName) ? profile.Id : profile.DisplayName,
            profileUrl,
            avatarUrl);
    }

    public async Task<ExternalTokenResponse> RefreshTokenAsync(
        string refreshToken,
        CancellationToken ct)
    {
        EnsureRequiredAuthSettings();

        using var request = new HttpRequestMessage(HttpMethod.Post, ResolveTokenEndpoint())
        {
            Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["grant_type"] = "refresh_token",
                ["refresh_token"] = refreshToken,
                ["client_id"] = _settings.ClientId
            })
        };

        using var response = await _httpClient.SendAsync(request, ct);
        var body = await response.Content.ReadAsStringAsync(ct);

        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException($"Spotify token refresh failed with status {(int)response.StatusCode}: {body}");

        var token = JsonSerializer.Deserialize<SpotifyTokenResponse>(body, JsonOptions)
                    ?? throw new InvalidOperationException("Spotify refresh token response was empty.");

        if (string.IsNullOrWhiteSpace(token.AccessToken))
            throw new InvalidOperationException("Spotify refresh token response did not include an access token.");

        var expiresAt = token.ExpiresIn.HasValue
            ? DateTimeOffset.UtcNow.AddSeconds(token.ExpiresIn.Value)
            : (DateTimeOffset?)null;

        return new ExternalTokenResponse(
            token.AccessToken,
            token.RefreshToken,
            expiresAt,
            token.Scope);
    }

    private void EnsureRequiredAuthSettings()
    {
        if (string.IsNullOrWhiteSpace(_settings.ClientId))
            throw new InvalidOperationException("Spotify ClientId is not configured.");

        if (string.IsNullOrWhiteSpace(_settings.RedirectUri))
            throw new InvalidOperationException("Spotify RedirectUri is not configured.");
    }

    private void EnsureRequiredTokenSettings()
    {
        EnsureRequiredAuthSettings();
    }

    private string ResolveAuthorizationEndpoint() =>
        string.IsNullOrWhiteSpace(_settings.AuthorizationEndpoint)
            ? DefaultAuthorizationEndpoint
            : _settings.AuthorizationEndpoint;

    private string ResolveTokenEndpoint() =>
        string.IsNullOrWhiteSpace(_settings.TokenEndpoint)
            ? DefaultTokenEndpoint
            : _settings.TokenEndpoint;

    private string ResolveCurrentUserEndpoint() =>
        string.IsNullOrWhiteSpace(_settings.CurrentUserEndpoint)
            ? DefaultCurrentUserEndpoint
            : _settings.CurrentUserEndpoint;

    private string[] ResolveScopes() =>
        _settings.Scopes.Length == 0 ? DefaultScopes : _settings.Scopes;

    private sealed record SpotifyTokenResponse(
        [property: JsonPropertyName("access_token")] string AccessToken,
        [property: JsonPropertyName("refresh_token")] string? RefreshToken,
        [property: JsonPropertyName("expires_in")] int? ExpiresIn,
        [property: JsonPropertyName("scope")] string? Scope);

    private sealed record SpotifyUserProfileResponse(
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("display_name")] string? DisplayName,
        [property: JsonPropertyName("external_urls")] SpotifyExternalUrlsResponse? ExternalUrls,
        [property: JsonPropertyName("images")] SpotifyImageResponse[]? Images);

    private sealed record SpotifyExternalUrlsResponse(
        [property: JsonPropertyName("spotify")] string? Spotify);

    private sealed record SpotifyImageResponse(
        [property: JsonPropertyName("url")] string? Url,
        [property: JsonPropertyName("height")] int? Height);
}
