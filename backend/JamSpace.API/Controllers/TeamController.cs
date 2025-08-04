using JamSpace.API.Extensions;
using JamSpace.Application.Features.Teams.Commands.AcceptTeamInvite;
using JamSpace.Application.Features.Teams.Commands.CancelTeamInvite;
using JamSpace.Application.Features.Teams.Commands.ChangeTeamMemberFunctionalRole;
using JamSpace.Application.Features.Teams.Commands.ChangeTeamName;
using JamSpace.Application.Features.Teams.Commands.CreateTeam;
using JamSpace.Application.Features.Teams.Commands.DeleteTeam;
using JamSpace.Application.Features.Teams.Commands.EditTeamMemberMusicalRole;
using JamSpace.Application.Features.Teams.Commands.KickTeamMember;
using JamSpace.Application.Features.Teams.Commands.LeaveTeam;
using JamSpace.Application.Features.Teams.Commands.RejectTeamInvite;
using JamSpace.Application.Features.Teams.Commands.SendTeamInvite;
using JamSpace.Application.Features.Teams.Dtos;
using JamSpace.Application.Features.Teams.Queries.GetDetails;
using JamSpace.Application.Features.Teams.Queries.GetMyPendingInvites;
using JamSpace.Application.Features.Teams.Queries.GetMyTeams;
using JamSpace.Application.Features.Teams.Queries.GetTeamInvites;
using JamSpace.Application.Features.Uploads.UpdateTeamPicture;
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
    public async Task<ActionResult<TeamInviteDto>> SendInvite(string username, [FromBody] Guid teamId)
    {
        var invitingUserId = User.GetUserId();
        var inviteDto = await _mediator.Send(new SendTeamInviteCommand(username, teamId, invitingUserId));
        return Ok(inviteDto);
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
    public async Task<ActionResult<TeamInviteDto>> AcceptInvite(Guid inviteId)
    {
        var userId = User.GetUserId();
        var inviteDto = await _mediator.Send(new AcceptTeamInviteCommand(inviteId, userId));
        return Ok(inviteDto);
    }

    [HttpPost("invites/{inviteId}/reject")]
    [Authorize]
    public async Task<ActionResult<TeamInviteDto>> RejectInvite(Guid inviteId)
    {
        var userId = User.GetUserId();
        var invite = await _mediator.Send(new RejectTeamInviteCommand(inviteId, userId));
        return Ok(invite);
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

    [HttpGet("{teamId}/invites")]
    [Authorize]
    public async Task<ActionResult<TeamInviteDto>> GetTeamInvites(Guid teamId)
    {
        var requestingUserId = User.GetUserId();
        var invites =await _mediator.Send(new GetTeamInvitesQuery(teamId, requestingUserId));
        return Ok(invites);
    }

    [HttpPatch("invites/{inviteId}/cancel")]
    [Authorize]
    public async Task<ActionResult<TeamInviteDto>> CancelInvite(Guid inviteId)
    {
        var requestingUserId = User.GetUserId();
        var inviteDto = await _mediator.Send(new CancelTeamInviteCommand(inviteId, requestingUserId));
        return Ok(inviteDto);
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

    [HttpDelete("{teamId}/members/leave")]
    [Authorize]
    public async Task<IActionResult> LeaveTeam(Guid teamId)
    {
        var requestingUserId = User.GetUserId();
        await _mediator.Send(new LeaveTeamCommand(teamId, requestingUserId));
        return NoContent();
    }
}