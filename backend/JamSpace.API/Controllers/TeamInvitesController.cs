using JamSpace.API.Extensions;
using JamSpace.Application.Features.TeamInvites.Commands.AcceptTeamInvite;
using JamSpace.Application.Features.TeamInvites.Commands.CancelTeamInvite;
using JamSpace.Application.Features.TeamInvites.Commands.RejectTeamInvite;
using JamSpace.Application.Features.TeamInvites.Commands.SendTeamInvite;
using JamSpace.Application.Features.TeamInvites.DTOs;
using JamSpace.Application.Features.TeamInvites.Queries.GetMyPendingInvites;
using JamSpace.Application.Features.TeamInvites.Queries.GetTeamInvites;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JamSpace.API.Controllers;

[ApiController]
[Route("api/teams/invites")]
public class TeamInvitesController : ControllerBase
{
    private readonly IMediator _mediator;
    public TeamInvitesController(IMediator mediator) => _mediator = mediator;

    [HttpPost("{username}")]
    [Authorize]
    public async Task<ActionResult<TeamInviteDto>> SendInvite(string username, [FromBody] Guid teamId)
    {
        var invitingUserId = User.GetUserId();
        var inviteDto = await _mediator.Send(new SendTeamInviteCommand(username, teamId, invitingUserId));
        return Ok(inviteDto);
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<List<TeamInviteDto>>> GetMyInvites()
    {
        var userId = User.GetUserId();
        var invites = await _mediator.Send(new GetMyPendingInvitesQuery(userId));
        return Ok(invites);
    }

    [HttpPost("{inviteId}/accept")]
    [Authorize]
    public async Task<ActionResult<TeamInviteDto>> AcceptInvite(Guid inviteId)
    {
        var userId = User.GetUserId();
        var inviteDto = await _mediator.Send(new AcceptTeamInviteCommand(inviteId, userId));
        return Ok(inviteDto);
    }

    [HttpPost("{inviteId}/reject")]
    [Authorize]
    public async Task<ActionResult<TeamInviteDto>> RejectInvite(Guid inviteId)
    {
        var userId = User.GetUserId();
        var invite = await _mediator.Send(new RejectTeamInviteCommand(inviteId, userId));
        return Ok(invite);
    }

    [HttpPatch("{inviteId}/cancel")]
    [Authorize]
    public async Task<ActionResult<TeamInviteDto>> CancelInvite(Guid inviteId)
    {
        var requestingUserId = User.GetUserId();
        var inviteDto = await _mediator.Send(new CancelTeamInviteCommand(inviteId, requestingUserId));
        return Ok(inviteDto);
    }

    [HttpGet("team/{teamId}")]
    [Authorize]
    public async Task<ActionResult<List<TeamInviteDto>>> GetTeamInvites(Guid teamId)
    {
        var requestingUserId = User.GetUserId();
        var invites = await _mediator.Send(new GetTeamInvitesQuery(teamId, requestingUserId));
        return Ok(invites);
    }
}