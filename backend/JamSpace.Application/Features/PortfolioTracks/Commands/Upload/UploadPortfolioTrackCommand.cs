using JamSpace.Application.Common.Models;
using JamSpace.Application.Features.PortfolioTracks.DTOs;
using MediatR;

namespace JamSpace.Application.Features.PortfolioTracks.Commands.Upload;

public sealed record UploadPortfolioTrackCommand(
    Guid UserId,
    string Title,
    string? ArtistName,
    string? AlbumTitle,
    int? DurationMs,
    FileUpload File,
    FileUpload? ArtworkFile
) : IRequest<PortfolioTrackDto>;
