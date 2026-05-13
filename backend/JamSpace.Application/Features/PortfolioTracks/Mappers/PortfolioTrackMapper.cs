using JamSpace.Application.Features.PortfolioTracks.DTOs;
using JamSpace.Domain.Entities;

namespace JamSpace.Application.Features.PortfolioTracks.Mappers;

public static class PortfolioTrackMapper
{
    public static PortfolioTrackDto ToDto(PortfolioTrack track) =>
        new(
            track.Id,
            track.UserId,
            track.Source.ToString(),
            track.Title,
            track.ArtistName,
            track.AlbumTitle,
            track.ArtworkUrl,
            track.DurationMs,
            track.ExternalTrackId,
            track.ExternalUrl,
            track.EmbedUrl,
            track.FileUrl,
            track.ContentType,
            track.Length,
            track.DisplayOrder,
            track.CreatedAt,
            track.UpdatedAt);
}
