namespace JamSpace.Application.Common.Models;

public sealed record ExternalTokenResponse(
    string AccessToken,
    string? RefreshToken,
    DateTimeOffset? ExpiresAt,
    string? Scope
);
