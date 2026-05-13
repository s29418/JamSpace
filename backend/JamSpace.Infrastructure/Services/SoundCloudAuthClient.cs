using System.Text.Json;
using System.Text.Json.Serialization;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Common.Models;
using JamSpace.Application.Common.Settings;
using JamSpace.Domain.Enums;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;

namespace JamSpace.Infrastructure.Services;

public class SoundCloudAuthClient : IMusicPlatformAuthClient
{
    private const string DefaultAuthorizationEndpoint = "https://secure.soundcloud.com/authorize";
    private const string DefaultTokenEndpoint = "https://secure.soundcloud.com/oauth/token";
    private const string DefaultCurrentUserEndpoint = "https://api.soundcloud.com/me";

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly HttpClient _httpClient;
    private readonly OAuthProviderSettings _settings;

    public SoundCloudAuthClient(HttpClient httpClient, IOptions<MusicPlatformOAuthSettings> settings)
    {
        _httpClient = httpClient;
        _settings = settings.Value.SoundCloud;
    }

    public ExternalMusicProvider Provider => ExternalMusicProvider.SoundCloud;

    public ExternalAuthUrl BuildAuthorizationUrl(string state, string codeChallenge)
    {
        EnsureRequiredAuthSettings();

        var query = new Dictionary<string, string?>
        {
            ["response_type"] = "code",
            ["client_id"] = _settings.ClientId,
            ["redirect_uri"] = _settings.RedirectUri,
            ["state"] = state,
            ["code_challenge"] = codeChallenge,
            ["code_challenge_method"] = "S256"
        };

        var scopes = ResolveScopes();
        if (scopes.Length > 0)
            query["scope"] = string.Join(' ', scopes);

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
                ["client_id"] = _settings.ClientId,
                ["client_secret"] = _settings.ClientSecret,
                ["redirect_uri"] = _settings.RedirectUri,
                ["code_verifier"] = codeVerifier,
                ["code"] = code
            })
        };

        request.Headers.Accept.ParseAdd("application/json; charset=utf-8");

        using var response = await _httpClient.SendAsync(request, ct);
        var body = await response.Content.ReadAsStringAsync(ct);

        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException($"SoundCloud token exchange failed with status {(int)response.StatusCode}: {body}");

        var token = JsonSerializer.Deserialize<SoundCloudTokenResponse>(body, JsonOptions)
                    ?? throw new InvalidOperationException("SoundCloud token response was empty.");

        if (string.IsNullOrWhiteSpace(token.AccessToken))
            throw new InvalidOperationException("SoundCloud token response did not include an access token.");

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
        request.Headers.Accept.ParseAdd("application/json; charset=utf-8");
        request.Headers.TryAddWithoutValidation("Authorization", $"OAuth {accessToken}");

        using var response = await _httpClient.SendAsync(request, ct);
        var body = await response.Content.ReadAsStringAsync(ct);

        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException($"SoundCloud profile request failed with status {(int)response.StatusCode}: {body}");

        var profile = JsonSerializer.Deserialize<SoundCloudUserProfileResponse>(body, JsonOptions)
                      ?? throw new InvalidOperationException("SoundCloud profile response was empty.");

        if (!profile.Id.HasValue)
            throw new InvalidOperationException("SoundCloud profile response did not include a user id.");

        var externalUserId = profile.Id.Value.ToString();
        var displayName = FirstNonEmpty(profile.Username, profile.FullName, externalUserId);
        var profileUrl = FirstNonEmpty(profile.PermalinkUrl, $"https://soundcloud.com/{Uri.EscapeDataString(displayName)}");

        return new ExternalUserProfile(
            externalUserId,
            displayName,
            profileUrl,
            profile.AvatarUrl);
    }

    public async Task<ExternalTokenResponse> RefreshTokenAsync(
        string refreshToken,
        CancellationToken ct)
    {
        EnsureRequiredTokenSettings();

        using var request = new HttpRequestMessage(HttpMethod.Post, ResolveTokenEndpoint())
        {
            Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["grant_type"] = "refresh_token",
                ["client_id"] = _settings.ClientId,
                ["client_secret"] = _settings.ClientSecret,
                ["refresh_token"] = refreshToken
            })
        };

        request.Headers.Accept.ParseAdd("application/json; charset=utf-8");

        using var response = await _httpClient.SendAsync(request, ct);
        var body = await response.Content.ReadAsStringAsync(ct);

        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException($"SoundCloud token refresh failed with status {(int)response.StatusCode}: {body}");

        var token = JsonSerializer.Deserialize<SoundCloudTokenResponse>(body, JsonOptions)
                    ?? throw new InvalidOperationException("SoundCloud refresh token response was empty.");

        if (string.IsNullOrWhiteSpace(token.AccessToken))
            throw new InvalidOperationException("SoundCloud refresh token response did not include an access token.");

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
            throw new InvalidOperationException("SoundCloud ClientId is not configured.");

        if (string.IsNullOrWhiteSpace(_settings.RedirectUri))
            throw new InvalidOperationException("SoundCloud RedirectUri is not configured.");
    }

    private void EnsureRequiredTokenSettings()
    {
        EnsureRequiredAuthSettings();

        if (string.IsNullOrWhiteSpace(_settings.ClientSecret))
            throw new InvalidOperationException("SoundCloud ClientSecret is not configured.");
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

    private string[] ResolveScopes() => _settings.Scopes;

    private static string FirstNonEmpty(params string?[] values)
    {
        return values.First(x => !string.IsNullOrWhiteSpace(x))!;
    }

    private sealed record SoundCloudTokenResponse(
        [property: JsonPropertyName("access_token")] string AccessToken,
        [property: JsonPropertyName("refresh_token")] string? RefreshToken,
        [property: JsonPropertyName("expires_in")] int? ExpiresIn,
        [property: JsonPropertyName("scope")] string? Scope);

    private sealed record SoundCloudUserProfileResponse(
        [property: JsonPropertyName("id")] long? Id,
        [property: JsonPropertyName("username")] string? Username,
        [property: JsonPropertyName("full_name")] string? FullName,
        [property: JsonPropertyName("permalink_url")] string? PermalinkUrl,
        [property: JsonPropertyName("avatar_url")] string? AvatarUrl);
}
