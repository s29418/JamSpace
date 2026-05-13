using JamSpace.Application.Common.Models;
using JamSpace.Domain.Enums;

namespace JamSpace.Application.Common.Interfaces;

public interface IMusicPlatformAuthClient
{
    ExternalMusicProvider Provider { get; }

    ExternalAuthUrl BuildAuthorizationUrl(string state, string codeChallenge);

    Task<ExternalTokenResponse> ExchangeCodeAsync(
        string code,
        string codeVerifier,
        CancellationToken ct);

    Task<ExternalTokenResponse> RefreshTokenAsync(
        string refreshToken,
        CancellationToken ct);

    Task<ExternalUserProfile> GetCurrentUserProfileAsync(
        string accessToken,
        CancellationToken ct);
}
