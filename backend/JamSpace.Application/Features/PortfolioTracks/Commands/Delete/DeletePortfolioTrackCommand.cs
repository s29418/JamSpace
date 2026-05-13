using MediatR;

namespace JamSpace.Application.Features.PortfolioTracks.Commands.Delete;

public sealed record DeletePortfolioTrackCommand(Guid UserId, Guid TrackId) : IRequest;
