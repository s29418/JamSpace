using JamSpace.API.Extensions;
using JamSpace.API.Requests;
using JamSpace.Application.Features.PortfolioTracks.Commands.AddExternalLink;
using JamSpace.Application.Features.PortfolioTracks.Commands.Delete;
using JamSpace.Application.Features.PortfolioTracks.Commands.Upload;
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

    [HttpPost("/api/me/portfolio/tracks/external-link")]
    [Authorize]
    public async Task<ActionResult<PortfolioTrackDto>> AddExternalPortfolioTrack(
        [FromBody] AddExternalPortfolioTrackRequest request,
        CancellationToken ct)
    {
        var userId = User.GetUserId();
        var result = await _mediator.Send(
            new AddExternalPortfolioTrackCommand(
                userId,
                request.Source,
                request.Title,
                request.ArtistName,
                request.AlbumTitle,
                request.ArtworkUrl,
                request.DurationMs,
                request.ExternalTrackId,
                request.ExternalUrl,
                request.EmbedUrl),
            ct);

        return StatusCode(StatusCodes.Status201Created, result);
    }

    [HttpPost("/api/me/portfolio/tracks/upload")]
    [Authorize]
    public async Task<ActionResult<PortfolioTrackDto>> UploadPortfolioTrack(
        [FromForm] UploadPortfolioTrackRequest request,
        CancellationToken ct)
    {
        if (request.File is null)
            return BadRequest(new { message = "File is required." });

        var userId = User.GetUserId();
        var result = await _mediator.Send(
            new UploadPortfolioTrackCommand(
                userId,
                request.Title,
                request.ArtistName,
                request.AlbumTitle,
                request.DurationMs,
                request.File.ToFileUpload(),
                request.ArtworkFile?.ToFileUpload()),
            ct);

        return StatusCode(StatusCodes.Status201Created, result);
    }

    [HttpDelete("/api/me/portfolio/tracks/{trackId:guid}")]
    [Authorize]
    public async Task<IActionResult> DeletePortfolioTrack([FromRoute] Guid trackId, CancellationToken ct)
    {
        var userId = User.GetUserId();
        await _mediator.Send(new DeletePortfolioTrackCommand(userId, trackId), ct);
        return NoContent();
    }
}
