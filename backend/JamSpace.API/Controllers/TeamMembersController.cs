using JamSpace.API.Extensions;
using JamSpace.Application.Features.TeamMembers.Commands.ChangeTeamMemberFunctionalRole;
using JamSpace.Application.Features.TeamMembers.Commands.EditTeamMemberMusicalRole;
using JamSpace.Application.Features.TeamMembers.Commands.KickTeamMember;
using JamSpace.Application.Features.TeamMembers.Commands.LeaveTeam;
using JamSpace.Application.Features.TeamMembers.DTOs;
using JamSpace.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JamSpace.API.Controllers;

[ApiController]
[Route("api/teams/{teamId}/members")]
public class TeamMembersController : ControllerBase
{
    private readonly IMediator _mediator;
    public TeamMembersController(IMediator mediator) => _mediator = mediator;

    [HttpPatch("{userId}/role")]
    [Authorize]
    public async Task<ActionResult<TeamMemberDto>> ChangeTeamMemberRole(
        Guid teamId, Guid userId, [FromQuery] FunctionalRole newRole, CancellationToken ct)
    {
        var requestingUserId = User.GetUserId();
        var updated = await _mediator.Send(new ChangeTeamMemberFunctionalRoleCommand(
            teamId, requestingUserId, userId, newRole), ct);
        return Ok(updated);
    }
    
    [HttpPatch("{userId}/musical-role")]
    [Authorize]
    public async Task<ActionResult<TeamMemberDto>> EditTeamMemberMusicalRole(
        Guid teamId, Guid userId, [FromQuery] string musicalRole, CancellationToken ct)
    {
        var requestingUserId = User.GetUserId();
        var updated = await _mediator.Send(new EditTeamMemberMusicalRoleCommand(
            teamId, requestingUserId, userId, musicalRole), ct);
        return Ok(updated);
    }

    [HttpDelete("{userId}")]
    [Authorize]
    public async Task<IActionResult> KickTeamMember(Guid teamId, Guid userId, CancellationToken ct)
    {
        var requestingUserId = User.GetUserId();
        await _mediator.Send(new KickTeamMemberCommand(teamId, requestingUserId, userId), ct);
        return NoContent();
    }

    [HttpDelete("leave")]
    [Authorize]
    public async Task<IActionResult> LeaveTeam(Guid teamId, CancellationToken ct)
    {
        var requestingUserId = User.GetUserId();
        await _mediator.Send(new LeaveTeamCommand(teamId, requestingUserId), ct);
        return NoContent();
    }
}