namespace JamSpace.Application.Features.ExternalAccounts.DTOs;

public sealed record SpotifyPlaylistDto(
    string Id,
    string Name,
    string ExternalUrl,
    string EmbedUrl,
    string? ImageUrl
);
