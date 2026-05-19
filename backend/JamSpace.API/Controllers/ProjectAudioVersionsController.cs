using JamSpace.API.Extensions;
using JamSpace.API.Requests;
using JamSpace.Application.Features.ProjectAudioVersions.Commands.Delete;
using JamSpace.Application.Features.ProjectAudioVersions.Commands.Upload;
using JamSpace.Application.Features.ProjectAudioVersions.DTOs;
using JamSpace.Application.Features.ProjectAudioVersions.Queries.GetProjectAudioVersions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JamSpace.API.Controllers;

[ApiController]
[Route("api/teams/{teamId}/projects/{projectId}/versions")]
public class ProjectAudioVersionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProjectAudioVersionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IReadOnlyList<ProjectAudioVersionDto>>> GetProjectAudioVersions(
        [FromRoute] Guid teamId,
        [FromRoute] Guid projectId,
        CancellationToken ct = default)
    {
        var userId = User.GetUserId();
        var result = await _mediator.Send(new GetProjectAudioVersionsQuery(teamId, projectId, userId), ct);
        return Ok(result);
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<ProjectAudioVersionDto>> UploadProjectAudioVersion(
        [FromRoute] Guid teamId,
        [FromRoute] Guid projectId,
        [FromForm] UploadProjectAudioVersionRequest request,
        CancellationToken ct = default)
    {
        if (request.File is null)
            return BadRequest(new { message = "File is required." });

        var userId = User.GetUserId();
        var result = await _mediator.Send(
            new UploadProjectAudioVersionCommand(
                teamId,
                projectId,
                userId,
                request.Name,
                request.DurationSeconds,
                request.File.ToFileUpload()),
            ct);

        return StatusCode(StatusCodes.Status201Created, result);
    }

    [HttpDelete("{versionId}")]
    [Authorize]
    public async Task<IActionResult> DeleteProjectAudioVersion(
        [FromRoute] Guid teamId,
        [FromRoute] Guid projectId,
        [FromRoute] Guid versionId,
        CancellationToken ct = default)
    {
        var userId = User.GetUserId();
        await _mediator.Send(new DeleteProjectAudioVersionCommand(teamId, projectId, versionId, userId), ct);
        return NoContent();
    }
}
