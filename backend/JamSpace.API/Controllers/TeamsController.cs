using JamSpace.API.Extensions;
using JamSpace.Application.Features.Teams.Commands.ChangeTeamName;
using JamSpace.Application.Features.Teams.Commands.CreateTeam;
using JamSpace.Application.Features.Teams.Commands.DeleteTeam;
using JamSpace.Application.Features.Teams.DTOs;
using JamSpace.Application.Features.Teams.Queries.GetMyTeams;
using JamSpace.Application.Features.Teams.Queries.GetTeamById;
using JamSpace.Application.Features.Uploads.UpdateTeamPicture;
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
    public async Task<ActionResult<TeamDto>> CreateTeam([FromBody] CreateTeamCommand cmd)
    {
        var userId = User.GetUserId();
        var result = await _mediator.Send(new CreateTeamWithUserCommand(cmd, userId));
        return Ok(result);
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<TeamDto>> GetTeamById(Guid id)
    {
        var userId = User.GetUserId();
        var result = await _mediator.Send(new GetTeamByIdQuery(id, userId));
        return Ok(result);
    }

    [HttpGet("my")]
    [Authorize]
    public async Task<ActionResult<List<TeamDto>>> GetMyTeams()
    {
        var userId = User.GetUserId();
        var result = await _mediator.Send(new GetMyTeamsQuery(userId));
        return Ok(result);
    }

    [HttpPatch("{teamId}/teamName")]
    [Authorize]
    public async Task<ActionResult<TeamDto>> ChangeTeamName(Guid teamId, [FromQuery] string teamName)
    {
        var requestingUserId = User.GetUserId();
        var updated = await _mediator.Send(new ChangeTeamNameCommand(teamId, requestingUserId, teamName));
        return Ok(updated);
    }

    [HttpDelete("{teamId}")]
    [Authorize]
    public async Task<IActionResult> DeleteTeam(Guid teamId)
    {
        var requestingUserId = User.GetUserId();
        await _mediator.Send(new DeleteTeamCommand(teamId, requestingUserId));
        return NoContent();
    }

    [HttpPatch("{teamId}/team-picture")]
    [Authorize]
    public async Task<ActionResult<string>> UpdateTeamPicture(Guid teamId, [FromForm] UpdateTeamPictureRequest request)
    {
        if (request.File.Length == 0)
            return BadRequest("No file provided.");

        var requestingUserId = User.GetUserId();

        var command = new UpdateTeamPictureCommand
        {
            TeamId = teamId,
            RequestingUserId = requestingUserId,
            File = request.File
        };

        var url = await _mediator.Send(command);
        return Ok(new { url });
    }
}