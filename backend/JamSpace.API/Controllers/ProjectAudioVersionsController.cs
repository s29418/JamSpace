using JamSpace.API.Extensions;
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
}