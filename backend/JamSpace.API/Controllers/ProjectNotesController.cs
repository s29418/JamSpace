using JamSpace.API.Extensions;
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
}
