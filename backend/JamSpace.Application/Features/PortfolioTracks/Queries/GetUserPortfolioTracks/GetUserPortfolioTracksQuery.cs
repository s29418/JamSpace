using JamSpace.Application.Features.PortfolioTracks.DTOs;
using MediatR;

namespace JamSpace.Application.Features.PortfolioTracks.Queries.GetUserPortfolioTracks;

public sealed record GetUserPortfolioTracksQuery(Guid UserId) : IRequest<IReadOnlyList<PortfolioTrackDto>>;
