using JamSpace.Application.Features.PortfolioTracks.DTOs;
using JamSpace.Domain.Enums;
using MediatR;

namespace JamSpace.Application.Features.PortfolioTracks.Commands.AddExternalLink;

public sealed record AddExternalPortfolioTrackCommand(
    Guid UserId,
    PortfolioTrackSource Source,
    string Title,
    string? ArtistName,
    string? AlbumTitle,
    string? ArtworkUrl,
    int? DurationMs,
    string? ExternalTrackId,
    string ExternalUrl,
    string? EmbedUrl
) : IRequest<PortfolioTrackDto>;
