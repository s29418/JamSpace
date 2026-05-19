using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Features.ExternalAccounts.DTOs;
using JamSpace.Domain.Enums;
using MediatR;

namespace JamSpace.Application.Features.ExternalAccounts.Queries.GetMySpotifyPlaylists;

public sealed class GetMySpotifyPlaylistsHandler
    : IRequestHandler<GetMySpotifyPlaylistsQuery, IReadOnlyList<SpotifyPlaylistDto>>
{
    private readonly IUserExternalAccountRepository _externalAccounts;
    private readonly IExternalAccountTokenService _tokenService;
    private readonly ISpotifyPlaylistClient _spotifyPlaylistClient;

    public GetMySpotifyPlaylistsHandler(
        IUserExternalAccountRepository externalAccounts,
        IExternalAccountTokenService tokenService,
        ISpotifyPlaylistClient spotifyPlaylistClient)
    {
        _externalAccounts = externalAccounts;
        _tokenService = tokenService;
        _spotifyPlaylistClient = spotifyPlaylistClient;
    }

    public async Task<IReadOnlyList<SpotifyPlaylistDto>> Handle(
        GetMySpotifyPlaylistsQuery request,
        CancellationToken cancellationToken)
    {
        var account = await _externalAccounts.GetActiveByUserAndProviderAsync(
            request.UserId,
            ExternalMusicProvider.Spotify,
            cancellationToken);

        if (account is null)
            throw new NotFoundException("Spotify account is not connected.");

        var accessToken = await _tokenService.GetValidAccessTokenAsync(account, cancellationToken);

        return await _spotifyPlaylistClient.GetCurrentUserPublicPlaylistsAsync(
            accessToken,
            cancellationToken);
    }
}
