using JamSpace.API.Extensions;
using JamSpace.API.Requests;
using JamSpace.Application.Features.TeamEvents.Commands.Create;
using JamSpace.Application.Features.TeamEvents.Commands.Delete;
using JamSpace.Application.Features.TeamEvents.Commands.Edit;
using JamSpace.Application.Features.TeamEvents.DTOs;
using JamSpace.Application.Features.TeamEvents.Queries.GetTeamEvents;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JamSpace.API.Controllers;

[ApiController]
[Route("api/teams/{teamId}/events")]
public class TeamEventController : ControllerBase
{
    private readonly IMediator _mediator;

    public TeamEventController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IReadOnlyList<TeamEventDto>>> GetEvents([FromRoute] Guid teamId, 
        [FromQuery] DateTimeOffset from, [FromQuery] DateTimeOffset to, CancellationToken ct = default)
    {
        var userId = User.GetUserId();
        var result = await _mediator.Send(new GetTeamEventsQuery(teamId, userId, from, to), ct);
        return Ok(result);
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<TeamEventDto>> CreateEvent([FromRoute] Guid teamId, 
        [FromBody] CreateTeamEventRequest request, CancellationToken ct = default)
    {
        var userId = User.GetUserId();
        var result = await _mediator.Send(new CreateTeamEventCommand(userId, teamId, request.Title, 
                request.Description, request.StartDateTime, request.DurationMinutes), ct);
        return StatusCode(StatusCodes.Status201Created, result);
    }

    [HttpPut("{eventId}")]
    [Authorize]
    public async Task<ActionResult<TeamEventDto>> EditEvent([FromRoute] Guid teamId, [FromRoute] Guid eventId, 
        [FromBody] EditTeamEventRequest request, CancellationToken ct = default)
    {
        var userId = User.GetUserId();
        var result = await _mediator.Send(new EditTeamEventCommand(eventId, teamId, userId, request.Title,
                request.Description, request.StartDateTime, request.DurationMinutes), ct);
        return Ok(result);
    }

    [HttpDelete("{eventId}")]
    [Authorize]
    public async Task<IActionResult> DeleteEvent([FromRoute] Guid teamId, [FromRoute] Guid eventId, 
        CancellationToken ct = default)
    {
        var userId = User.GetUserId();
        await _mediator.Send(new DeleteTeamEventCommand(eventId, teamId, userId), ct);
        return NoContent();
    }
}