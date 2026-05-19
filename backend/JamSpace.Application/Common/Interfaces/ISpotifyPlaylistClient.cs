using JamSpace.Application.Features.ExternalAccounts.DTOs;

namespace JamSpace.Application.Common.Interfaces;

public interface ISpotifyPlaylistClient
{
    Task<IReadOnlyList<SpotifyPlaylistDto>> GetCurrentUserPublicPlaylistsAsync(
        string accessToken,
        CancellationToken ct);
}
