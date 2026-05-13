using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Common.Persistence;
using MediatR;

namespace JamSpace.Application.Features.PortfolioTracks.Commands.Delete;

public class DeletePortfolioTrackHandler : IRequestHandler<DeletePortfolioTrackCommand>
{
    private readonly IPortfolioTrackRepository _portfolioTracks;
    private readonly IUnitOfWork _uow;

    public DeletePortfolioTrackHandler(
        IPortfolioTrackRepository portfolioTracks,
        IUnitOfWork uow)
    {
        _portfolioTracks = portfolioTracks;
        _uow = uow;
    }

    public async Task Handle(DeletePortfolioTrackCommand request, CancellationToken cancellationToken)
    {
        var track = await _portfolioTracks.GetByIdAndUserIdAsync(
            request.TrackId,
            request.UserId,
            cancellationToken);

        if (track is null)
            throw new NotFoundException("Portfolio track not found.");

        _portfolioTracks.SoftDelete(track, DateTimeOffset.UtcNow);
        await _uow.SaveChangesAsync(cancellationToken);
    }
}
