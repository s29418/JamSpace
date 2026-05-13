using JamSpace.Application.Features.PortfolioTracks.DTOs;
using JamSpace.Application.Features.PortfolioTracks.Queries.GetUserPortfolioTracks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JamSpace.API.Controllers;

[ApiController]
[Route("api/users/{userId:guid}/portfolio/tracks")]
public class PortfolioTracksController : ControllerBase
{
    private readonly IMediator _mediator;

    public PortfolioTracksController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IReadOnlyList<PortfolioTrackDto>>> GetUserPortfolioTracks(
        [FromRoute] Guid userId,
        CancellationToken ct)
    {
        var result = await _mediator.Send(new GetUserPortfolioTracksQuery(userId), ct);
        return Ok(result);
    }
}
