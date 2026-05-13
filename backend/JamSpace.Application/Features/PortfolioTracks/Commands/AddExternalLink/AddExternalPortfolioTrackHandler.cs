using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Common.Persistence;
using JamSpace.Application.Features.PortfolioTracks.DTOs;
using JamSpace.Application.Features.PortfolioTracks.Mappers;
using JamSpace.Domain.Entities;
using JamSpace.Domain.Enums;
using MediatR;

namespace JamSpace.Application.Features.PortfolioTracks.Commands.AddExternalLink;

public class AddExternalPortfolioTrackHandler
    : IRequestHandler<AddExternalPortfolioTrackCommand, PortfolioTrackDto>
{
    private readonly IUserRepository _users;
    private readonly IPortfolioTrackRepository _portfolioTracks;
    private readonly IUnitOfWork _uow;

    public AddExternalPortfolioTrackHandler(
        IUserRepository users,
        IPortfolioTrackRepository portfolioTracks,
        IUnitOfWork uow)
    {
        _users = users;
        _portfolioTracks = portfolioTracks;
        _uow = uow;
    }

    public async Task<PortfolioTrackDto> Handle(
        AddExternalPortfolioTrackCommand request,
        CancellationToken cancellationToken)
    {
        if (request.Source is PortfolioTrackSource.Upload)
            throw new InvalidOperationException("Upload source is not supported by this endpoint.");

        if (!await _users.ExistsAsync(request.UserId, cancellationToken))
            throw new NotFoundException("User not found.");

        var now = DateTimeOffset.UtcNow;
        var displayOrder = await _portfolioTracks.GetNextDisplayOrderAsync(request.UserId, cancellationToken);

        var track = new PortfolioTrack
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            Source = request.Source,
            Title = request.Title.Trim(),
            ArtistName = Normalize(request.ArtistName),
            AlbumTitle = Normalize(request.AlbumTitle),
            ArtworkUrl = Normalize(request.ArtworkUrl),
            DurationMs = request.DurationMs,
            ExternalTrackId = Normalize(request.ExternalTrackId),
            ExternalUrl = request.ExternalUrl.Trim(),
            EmbedUrl = Normalize(request.EmbedUrl),
            DisplayOrder = displayOrder,
            CreatedAt = now,
            UpdatedAt = now
        };

        await _portfolioTracks.AddAsync(track, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        return PortfolioTrackMapper.ToDto(track);
    }

    private static string? Normalize(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
