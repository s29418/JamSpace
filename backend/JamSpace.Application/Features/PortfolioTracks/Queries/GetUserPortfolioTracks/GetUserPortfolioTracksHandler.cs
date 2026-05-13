using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Features.PortfolioTracks.DTOs;
using JamSpace.Application.Features.PortfolioTracks.Mappers;
using MediatR;

namespace JamSpace.Application.Features.PortfolioTracks.Queries.GetUserPortfolioTracks;

public class GetUserPortfolioTracksHandler
    : IRequestHandler<GetUserPortfolioTracksQuery, IReadOnlyList<PortfolioTrackDto>>
{
    private readonly IUserRepository _users;
    private readonly IPortfolioTrackRepository _portfolioTracks;

    public GetUserPortfolioTracksHandler(
        IUserRepository users,
        IPortfolioTrackRepository portfolioTracks)
    {
        _users = users;
        _portfolioTracks = portfolioTracks;
    }

    public async Task<IReadOnlyList<PortfolioTrackDto>> Handle(
        GetUserPortfolioTracksQuery request,
        CancellationToken cancellationToken)
    {
        if (!await _users.ExistsAsync(request.UserId, cancellationToken))
            throw new NotFoundException("User not found.");

        var tracks = await _portfolioTracks.GetByUserIdAsync(request.UserId, cancellationToken);
        return tracks.Select(PortfolioTrackMapper.ToDto).ToList();
    }
}
