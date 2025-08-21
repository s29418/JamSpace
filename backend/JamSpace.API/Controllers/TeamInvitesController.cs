using JamSpace.API.Extensions;
using JamSpace.Application.Features.TeamInvites.Commands.AcceptTeamInvite;
using JamSpace.Application.Features.TeamInvites.Commands.CancelTeamInvite;
using JamSpace.Application.Features.TeamInvites.Commands.RejectTeamInvite;
using JamSpace.Application.Features.TeamInvites.Commands.SendTeamInvite;
using JamSpace.Application.Features.TeamInvites.DTOs;
using JamSpace.Application.Features.TeamInvites.Queries.GetMyPendingInvites;
using JamSpace.Application.Features.TeamInvites.Queries.GetTeamInviteById;
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
    public async Task<ActionResult<TeamInviteDto>> SendInvite(string username, [FromBody] Guid teamId, CancellationToken ct)
    {
        var invitingUserId = User.GetUserId();
        var inviteDto = await _mediator.Send(new SendTeamInviteCommand(username, teamId, invitingUserId), ct);
        return CreatedAtRoute("GetTeamInviteById", new { id = inviteDto.Id }, inviteDto);
    }

    [HttpGet("{id:guid}", Name = "GetTeamInviteById")]
    [Authorize]
    public async Task<ActionResult<TeamInviteDto>> GetTeamInviteById(Guid id, CancellationToken ct)
    {
        var dto = await _mediator.Send(new GetTeamInviteByIdQuery(id), ct);
        return Ok(dto);
    }
    
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<List<TeamInviteDto>>> GetMyInvites(CancellationToken ct)
    {
        var userId = User.GetUserId();
        var invites = await _mediator.Send(new GetMyPendingInvitesQuery(userId), ct);
        return Ok(invites);
    }
    
    [HttpGet("team/{teamId}")]
    [Authorize]
    public async Task<ActionResult<List<TeamInviteDto>>> GetTeamInvites(Guid teamId, CancellationToken ct)
    {
        var requestingUserId = User.GetUserId();
        var invites = await _mediator.Send(new GetTeamInvitesQuery(teamId, requestingUserId), ct);
        return Ok(invites);
    }

    [HttpPatch("{inviteId}/accept")]
    [Authorize]
    public async Task<ActionResult<TeamInviteDto>> AcceptInvite(Guid inviteId, CancellationToken ct)
    {
        var userId = User.GetUserId();
        var inviteDto = await _mediator.Send(new AcceptTeamInviteCommand(inviteId, userId), ct);
        return Ok(inviteDto);
    }

    [HttpPatch("{inviteId}/reject")]
    [Authorize]
    public async Task<ActionResult<TeamInviteDto>> RejectInvite(Guid inviteId, CancellationToken ct)
    {
        var userId = User.GetUserId();
        var invite = await _mediator.Send(new RejectTeamInviteCommand(inviteId, userId), ct);
        return Ok(invite);
    }

    [HttpPatch("{inviteId}/cancel")]
    [Authorize]
    public async Task<ActionResult<TeamInviteDto>> CancelInvite(Guid inviteId, CancellationToken ct)
    {
        var requestingUserId = User.GetUserId();
        var inviteDto = await _mediator.Send(new CancelTeamInviteCommand(inviteId, requestingUserId), ct);
        return Ok(inviteDto);
    }
}