using JamSpace.Application.Common.Enums;
using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Common.Persistence;
using JamSpace.Application.Features.PortfolioTracks.DTOs;
using JamSpace.Application.Features.PortfolioTracks.Mappers;
using JamSpace.Domain.Entities;
using JamSpace.Domain.Enums;
using MediatR;

namespace JamSpace.Application.Features.PortfolioTracks.Commands.Upload;

public class UploadPortfolioTrackHandler : IRequestHandler<UploadPortfolioTrackCommand, PortfolioTrackDto>
{
    private readonly IUserRepository _users;
    private readonly IPortfolioTrackRepository _portfolioTracks;
    private readonly IFileStorageService _fileStorage;
    private readonly IUnitOfWork _uow;

    public UploadPortfolioTrackHandler(
        IUserRepository users,
        IPortfolioTrackRepository portfolioTracks,
        IFileStorageService fileStorage,
        IUnitOfWork uow)
    {
        _users = users;
        _portfolioTracks = portfolioTracks;
        _fileStorage = fileStorage;
        _uow = uow;
    }

    public async Task<PortfolioTrackDto> Handle(
        UploadPortfolioTrackCommand request,
        CancellationToken cancellationToken)
    {
        if (!await _users.ExistsAsync(request.UserId, cancellationToken))
            throw new NotFoundException("User not found.");

        string? uploadedUrl = null;
        string? artworkUrl = null;
        var trackId = Guid.NewGuid();

        try
        {
            uploadedUrl = await _fileStorage.UploadAsync(
                request.File,
                StorageObjectType.PortfolioTrackAudio,
                trackId,
                cancellationToken);

            if (request.ArtworkFile is not null)
            {
                artworkUrl = await _fileStorage.UploadAsync(
                    request.ArtworkFile,
                    StorageObjectType.PortfolioTrackArtwork,
                    trackId,
                    cancellationToken);
            }

            var now = DateTimeOffset.UtcNow;
            var displayOrder = await _portfolioTracks.GetNextDisplayOrderAsync(
                request.UserId,
                cancellationToken);

            var track = new PortfolioTrack
            {
                Id = trackId,
                UserId = request.UserId,
                Source = PortfolioTrackSource.Upload,
                Title = request.Title.Trim(),
                ArtistName = Normalize(request.ArtistName),
                AlbumTitle = Normalize(request.AlbumTitle),
                ArtworkUrl = artworkUrl,
                DurationMs = request.DurationMs,
                FileUrl = uploadedUrl,
                OriginalFileName = request.File.FileName,
                ContentType = request.File.ContentType ?? "application/octet-stream",
                Length = request.File.Length,
                DisplayOrder = displayOrder,
                CreatedAt = now,
                UpdatedAt = now
            };

            await _portfolioTracks.AddAsync(track, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return PortfolioTrackMapper.ToDto(track);
        }
        catch
        {
            if (!string.IsNullOrWhiteSpace(uploadedUrl))
            {
                try
                {
                    await _fileStorage.DeleteAsync(uploadedUrl, cancellationToken);
                }
                catch
                {
                    // ignored
                }
            }

            if (!string.IsNullOrWhiteSpace(artworkUrl))
            {
                try
                {
                    await _fileStorage.DeleteAsync(artworkUrl, cancellationToken);
                }
                catch
                {
                    // ignored
                }
            }

            throw;
        }
    }

    private static string? Normalize(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
