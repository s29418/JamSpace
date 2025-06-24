using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using JamSpace.Application.Features.Teams.Create;
using JamSpace.Application.Features.Teams.Dtos;
using JamSpace.Application.Features.Teams.GetDetails;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
    
}