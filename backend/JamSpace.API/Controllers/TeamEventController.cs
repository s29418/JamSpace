using JamSpace.API.Extensions;
using JamSpace.Application.Features.TeamEvents.Commands.Create;
using JamSpace.Application.Features.TeamEvents.DTOs;
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

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<TeamEventDto>> CreateEvent([FromRoute] Guid teamId, [FromBody] string title, 
        [FromBody] string? description, [FromBody] DateTimeOffset startDateTime, [FromBody] int durationMinutes, 
        CancellationToken ct = default)
    {
        var userId = User.GetUserId();
        var result =
            await _mediator.Send(
                new CreateTeamEventCommand(userId, teamId, title, description, startDateTime, durationMinutes), ct);

        return StatusCode(StatusCodes.Status201Created, result);
    }
    
}