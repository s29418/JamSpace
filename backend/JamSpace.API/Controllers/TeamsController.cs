using JamSpace.API.Extensions;
using JamSpace.API.Requests;
using JamSpace.Application.Features.Teams.Commands.ChangeTeamName;
using JamSpace.Application.Features.Teams.Commands.CreateTeam;
using JamSpace.Application.Features.Teams.Commands.DeleteTeam;
using JamSpace.Application.Features.Teams.Commands.UpdateTeamPicture;
using JamSpace.Application.Features.Teams.DTOs;
using JamSpace.Application.Features.Teams.Queries.GetMyTeams;
using JamSpace.Application.Features.Teams.Queries.GetTeamById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JamSpace.API.Controllers;

[ApiController]
[Route("api/teams")]
public class TeamsController : ControllerBase
{
    private readonly IMediator _mediator;
    public TeamsController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<TeamDto>> CreateTeam([FromBody] CreateTeamCommand command, CancellationToken ct)
    {
        var userId = User.GetUserId();
        var result = await _mediator.Send(new CreateTeamWithUserCommand(command, userId), ct);
        return CreatedAtRoute("GetTeamById", new { id = result.Id }, result);
    }

    [HttpGet("{id:guid}", Name = "GetTeamById")]
    [Authorize]
    public async Task<ActionResult<TeamDto>> GetTeamById(Guid id, CancellationToken ct)
    {
        var userId = User.GetUserId();
        var result = await _mediator.Send(new GetTeamByIdQuery(id, userId), ct);
        return Ok(result);
    }

    [HttpGet("my")]
    [Authorize]
    public async Task<ActionResult<List<TeamDto>>> GetMyTeams(CancellationToken ct)
    {
        var userId = User.GetUserId();
        var result = await _mediator.Send(new GetMyTeamsQuery(userId), ct);
        return Ok(result);
    }

    [HttpPatch("{teamId}/teamName")]
    [Authorize]
    public async Task<ActionResult<TeamDto>> ChangeTeamName(Guid teamId, [FromQuery] string teamName, CancellationToken ct)
    {
        var requestingUserId = User.GetUserId();
        var updated = await _mediator.Send(new ChangeTeamNameCommand(teamId, requestingUserId, teamName), ct);
        return Ok(updated);
    }
    
    [HttpPatch("{teamId}/team-picture")]
    [Authorize]
    public async Task<ActionResult<string>> UpdateTeamPicture(
        Guid teamId,
        [FromForm] UploadPictureRequest request,
        CancellationToken ct)
    {
        var requestingUserId = User.GetUserId();

        if (request.File is null)
            return BadRequest("File is required");

        var url = await _mediator.Send(new UpdateTeamPictureCommand(
            teamId,
            requestingUserId,
            request.File.ToFileUpload()
        ), ct);

        return Ok(new { url });
    }

    [HttpDelete("{teamId}")]
    [Authorize]
    public async Task<IActionResult> DeleteTeam(Guid teamId, CancellationToken ct)
    {
        var requestingUserId = User.GetUserId();
        await _mediator.Send(new DeleteTeamCommand(teamId, requestingUserId), ct);
        return NoContent();
    }
}