using JamSpace.API.Extensions;
using JamSpace.API.Requests;
using JamSpace.Application.Features.ProjectNotes.Commands.Complete;
using JamSpace.Application.Features.ProjectNotes.Commands.Create;
using JamSpace.Application.Features.ProjectNotes.Commands.Edit;
using JamSpace.Application.Features.ProjectNotes.Commands.Reopen;
using JamSpace.Application.Features.ProjectNotes.DTOs;
using JamSpace.Application.Features.ProjectNotes.Queries.GetProjectNotes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JamSpace.API.Controllers;

[ApiController]
[Route("api/teams/{teamId}/projects/{projectId}/notes")]
public class ProjectNotesController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProjectNotesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IReadOnlyList<ProjectNoteDto>>> GetProjectNotes(
        [FromRoute] Guid teamId,
        [FromRoute] Guid projectId,
        [FromQuery] Guid? versionId,
        CancellationToken ct = default)
    {
        var userId = User.GetUserId();
        var result = await _mediator.Send(new GetProjectNotesQuery(teamId, projectId, userId, versionId), ct);
        return Ok(result);
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<ProjectNoteDto>> CreateProjectNote(
        [FromRoute] Guid teamId,
        [FromRoute] Guid projectId,
        [FromBody] CreateProjectNoteRequest request,
        CancellationToken ct = default)
    {
        var userId = User.GetUserId();
        var result = await _mediator.Send(
            new CreateProjectNoteCommand(
                teamId,
                projectId,
                userId,
                request.Content,
                request.AudioVersionId,
                request.StartTimeSeconds,
                request.EndTimeSeconds),
            ct);

        return StatusCode(StatusCodes.Status201Created, result);
    }

    [HttpPut("{noteId}")]
    [Authorize]
    public async Task<ActionResult<ProjectNoteDto>> EditProjectNote(
        [FromRoute] Guid teamId,
        [FromRoute] Guid projectId,
        [FromRoute] Guid noteId,
        [FromBody] EditProjectNoteRequest request,
        CancellationToken ct = default)
    {
        var userId = User.GetUserId();
        var result = await _mediator.Send(
            new EditProjectNoteCommand(
                teamId,
                projectId,
                noteId,
                userId,
                request.Content,
                request.AudioVersionId,
                request.StartTimeSeconds,
                request.EndTimeSeconds),
            ct);

        return Ok(result);
    }

    [HttpPatch("{noteId}/complete")]
    [Authorize]
    public async Task<ActionResult<ProjectNoteDto>> CompleteProjectNote(
        [FromRoute] Guid teamId,
        [FromRoute] Guid projectId,
        [FromRoute] Guid noteId,
        CancellationToken ct = default)
    {
        var userId = User.GetUserId();
        var result = await _mediator.Send(new CompleteProjectNoteCommand(teamId, projectId, noteId, userId), ct);
        return Ok(result);
    }

    [HttpPatch("{noteId}/reopen")]
    [Authorize]
    public async Task<ActionResult<ProjectNoteDto>> ReopenProjectNote(
        [FromRoute] Guid teamId,
        [FromRoute] Guid projectId,
        [FromRoute] Guid noteId,
        CancellationToken ct = default)
    {
        var userId = User.GetUserId();
        var result = await _mediator.Send(new ReopenProjectNoteCommand(teamId, projectId, noteId, userId), ct);
        return Ok(result);
    }
}
