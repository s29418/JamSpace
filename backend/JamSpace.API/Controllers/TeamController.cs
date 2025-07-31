using JamSpace.API.Extensions;
using JamSpace.Application.Features.Teams.Commands.AcceptTeamInvite;
using JamSpace.Application.Features.Teams.Commands.CancelTeamInvite;
using JamSpace.Application.Features.Teams.Commands.ChangeTeamMemberFunctionalRole;
using JamSpace.Application.Features.Teams.Commands.ChangeTeamName;
using JamSpace.Application.Features.Teams.Commands.CreateTeam;
using JamSpace.Application.Features.Teams.Commands.DeleteTeam;
using JamSpace.Application.Features.Teams.Commands.EditTeamMemberMusicalRole;
using JamSpace.Application.Features.Teams.Commands.KickTeamMember;
using JamSpace.Application.Features.Teams.Commands.RejectTeamInvite;
using JamSpace.Application.Features.Teams.Commands.SendTeamInvite;
using JamSpace.Application.Features.Teams.Dtos;
using JamSpace.Application.Features.Teams.Queries.GetDetails;
using JamSpace.Application.Features.Teams.Queries.GetMyPendingInvites;
using JamSpace.Application.Features.Teams.Queries.GetMyTeams;
using JamSpace.Application.Features.Teams.Queries.GetTeamInvites;
using JamSpace.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JamSpace.API.Controllers;

[ApiController]
[Route("api/teams")]
public class TeamController : ControllerBase
{
    private readonly IMediator _mediator;

    public TeamController(IMediator mediator)
    {
        _mediator = mediator;
    }

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
    
    [HttpPost("invites/{username}")]
    [Authorize]
    public async Task<IActionResult> SendInvite(string username, [FromBody] Guid teamId)
    {
        var invitingUserId = User.GetUserId();
        await _mediator.Send(new SendTeamInviteCommand(username, teamId, invitingUserId));
        return Ok();
    }
    
    [HttpGet("invites")]
    [Authorize]
    public async Task<ActionResult<List<TeamInviteDto>>> GetMyInvites()
    {
        var userId = User.GetUserId();
        var invites = await _mediator.Send(new GetMyPendingInvitesQuery(userId));
        return Ok(invites);
    }
    
    [HttpPost("invites/{inviteId}/accept")]
    [Authorize]
    public async Task<IActionResult> AcceptInvite(Guid id)
    {
        var userId = User.GetUserId();
        await _mediator.Send(new AcceptTeamInviteCommand(id, userId));
        return Ok();
    }

    [HttpPost("invites/{inviteId}/reject")]
    [Authorize]
    public async Task<IActionResult> RejectInvite(Guid id)
    {
        var userId = User.GetUserId();
        await _mediator.Send(new RejectTeamInviteCommand(id, userId));
        return Ok();
    }

    [HttpPatch("{teamId}/members/{userId}/role")]
    [Authorize]
    public async Task<ActionResult<TeamMemberDto>> ChangeTeamMemberRole(Guid teamId, Guid userId, [FromQuery] FunctionalRole newRole)
    {
        var requestingUserId = User.GetUserId();
        var updated = await _mediator.Send(new ChangeTeamMemberFunctionalRoleCommand(teamId, requestingUserId, userId, newRole));
        return Ok(updated);
    }

    [HttpDelete("{teamId}/members/{userId}")]
    [Authorize]
    public async Task<IActionResult> KickTeamMember(Guid teamId, Guid userId)
    {
        var requestingUserId = User.GetUserId();
        await _mediator.Send(new KickTeamMemberCommand(teamId, requestingUserId, userId));
        return NoContent();
    }

    [HttpPatch("{teamId}/members/{userId}/musical-role")]
    [Authorize]
    public async Task<ActionResult<TeamMemberDto>> EditTeamMemberMusicalRole(Guid teamId, Guid userId,
        [FromQuery] string musicalRole)
    {
        var requestingUserId = User.GetUserId();
        var updated = await 
            _mediator.Send(new EditTeamMemberMusicalRoleCommand(teamId, requestingUserId, userId, musicalRole));
        return Ok(updated);
    }

    [HttpPatch("{teamId}/name")]
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

    [HttpGet("{teamId}/invites")]
    [Authorize]
    public async Task<ActionResult<TeamInviteDto>> GetTeamInvites(Guid teamId)
    {
        var requestingUserId = User.GetUserId();
        var invites =await _mediator.Send(new GetTeamInvitesQuery(teamId, requestingUserId));
        return Ok(invites);
    }

    [HttpPatch("/invites/{inviteId}/cancel")]
    [Authorize]
    public async Task<IActionResult> CancelInvite(Guid teamInviteId)
    {
        var requestingUserId = User.GetUserId();
        await _mediator.Send(new CancelTeamInviteCommand(teamInviteId, requestingUserId));
        return NoContent();
    }
    
}