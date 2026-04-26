using JamSpace.API.Extensions;
using JamSpace.API.Requests;
using JamSpace.Application.Features.Projects.Commands.Create;
using JamSpace.Application.Features.Projects.Commands.UploadProjectPicture;
using JamSpace.Application.Features.Projects.DTOs;
using JamSpace.Application.Features.Projects.Queries.GetTeamProjects;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JamSpace.API.Controllers;

[ApiController]
[Route("api/teams/{teamId}/projects")]
public class ProjectController : ControllerBase
{
    private readonly IMediator _mediator;
    public ProjectController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IReadOnlyList<ProjectDto>>> GetTeamProjects([FromRoute] Guid teamId, 
        CancellationToken ct)
    {
        var userId = User.GetUserId();
        var result = await _mediator.Send(new GetTeamProjectsQuery(userId, teamId), ct);
        return Ok(result);
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<ProjectDto>> CreateProject([FromRoute] Guid teamId, [FromBody] string name,
        CancellationToken ct = default)
    {
        var userId = User.GetUserId();
        var result = await _mediator.Send(new CreateProjectCommand(userId, teamId, name), ct);
        return StatusCode(StatusCodes.Status201Created, result);
    }

    [HttpPatch("{projectId}/picture")]
    [Authorize]
    public async Task<ActionResult<string>> UpdateProjectPicture([FromRoute] Guid projectId, [FromRoute] Guid teamId,
        [FromForm] UploadPictureRequest request, CancellationToken ct = default)
    {
        var userId = User.GetUserId();

        if (request.File is null)
            return BadRequest("File is required.");

        var url = await _mediator.Send(
            new UploadProjectPictureCommand(projectId, teamId, userId, request.File.ToFileUpload()), ct);

        return Ok(new { url });
    }
}