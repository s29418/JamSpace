using JamSpace.Application.Features.ExternalAccounts.DTOs;
using MediatR;

namespace JamSpace.Application.Features.ExternalAccounts.Queries.GetMySpotifyPlaylists;

public sealed record GetMySpotifyPlaylistsQuery(Guid UserId) : IRequest<IReadOnlyList<SpotifyPlaylistDto>>;
