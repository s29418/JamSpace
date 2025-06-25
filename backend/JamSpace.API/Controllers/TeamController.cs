using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using JamSpace.Application.Features.Teams.AcceptTeamInvite;
using JamSpace.Application.Features.Teams.Create;
using JamSpace.Application.Features.Teams.Dtos;
using JamSpace.Application.Features.Teams.GetDetails;
using JamSpace.Application.Features.Teams.GetMyPendingInvites;
using JamSpace.Application.Features.Teams.GetMyTeams;
using JamSpace.Application.Features.Teams.RejectTeamInvite;
using JamSpace.Application.Features.Teams.SendTeamInvite;
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
        var userIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                          ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userIdClaim is null)
            return Unauthorized();
        
        var userId = Guid.Parse(userIdClaim);

        var result = await _mediator.Send(new CreateTeamWithUserCommand(cmd, userId));
        return Ok(result);
    }
    
    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<TeamDto>> GetTeamById(Guid id)
    {
        var userIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                          ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userIdClaim is null)
            return Unauthorized();

        var userId = Guid.Parse(userIdClaim);

        var result = await _mediator.Send(new GetTeamByIdQuery(id, userId));
        return Ok(result);
    }
    
    [HttpGet("my")]
    [Authorize]
    public async Task<ActionResult<List<TeamDto>>> GetMyTeams()
    {
        var userIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                          ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userIdClaim is null)
            return Unauthorized();

        var userId = Guid.Parse(userIdClaim);
        var result = await _mediator.Send(new GetMyTeamsQuery(userId));

        return Ok(result);
    }
    
    [HttpPost("invite/{username}")]
    [Authorize]
    public async Task<IActionResult> SendInvite(string username, [FromBody] Guid teamId)
    {
        var userIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                          ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userIdClaim is null)
            return Unauthorized();

        var invitingUserId = Guid.Parse(userIdClaim);

        await _mediator.Send(new SendTeamInviteCommand(username, teamId, invitingUserId));
        return Ok();
    }
    
    [HttpGet("invite")]
    [Authorize]
    public async Task<ActionResult<List<TeamInviteDto>>> GetMyInvites()
    {
        var userIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                          ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userIdClaim is null)
            return Unauthorized();

        var userId = Guid.Parse(userIdClaim);

        var invites = await _mediator.Send(new GetMyPendingInvitesQuery(userId));
        return Ok(invites);
    }
    
    [HttpPost("invite/{id}/accept")]
    [Authorize]
    public async Task<IActionResult> AcceptInvite(Guid id)
    {
        var userIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                          ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userIdClaim is null)
            return Unauthorized();

        var userId = Guid.Parse(userIdClaim);
        await _mediator.Send(new AcceptTeamInviteCommand(id, userId));
        return Ok();
    }

    [HttpPost("invite/{id}/reject")]
    [Authorize]
    public async Task<IActionResult> RejectInvite(Guid id)
    {
        var userIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                          ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userIdClaim is null)
            return Unauthorized();

        var userId = Guid.Parse(userIdClaim);
        await _mediator.Send(new RejectTeamInviteCommand(id, userId));
        return Ok();
    }

    
}