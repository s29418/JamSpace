using JamSpace.API.Extensions;
using JamSpace.Application.Features.UserFollows.Commands.FollowUser;
using JamSpace.Application.Features.UserFollows.Commands.UnfollowUser;
using JamSpace.Application.Features.UserFollows.DTOs;
using JamSpace.Application.Features.UserFollows.Queries.GetUserFollowers;
using JamSpace.Application.Features.UserFollows.Queries.GetUserFollowing;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JamSpace.API.Controllers;

[ApiController]
[Route("api/users/{userId:guid}")]
public class UserFollowsController : ControllerBase
{
    private readonly IMediator _mediator;
    
    public UserFollowsController(IMediator mediator) => _mediator = mediator;
    
    [HttpPost("following")]
    [Authorize]
    public async Task<ActionResult<UserFollowDto>> FollowUser([FromRoute] Guid userId, [FromBody] Guid targetUserId, CancellationToken ct)
    {
        var authId = User.GetUserId();
        if (authId != userId) return Unauthorized("You can only modify your own follows.");
        
        var result = await _mediator.Send(new FollowUserCommand(userId, targetUserId), ct);
        return CreatedAtAction(nameof(GetUserFollowing), new { userId }, result);
    }

    [HttpDelete("following/{targetUserId:guid}")]
    [Authorize]
    public async Task<IActionResult> UnfollowUser([FromRoute] Guid userId, [FromRoute] Guid targetUserId, CancellationToken ct)
    {
        var authId = User.GetUserId();
        if (authId != userId) return Unauthorized("You can only modify your own follows.");

        await _mediator.Send(new UnfollowUserCommand(userId, targetUserId), ct);
        return NoContent();
    }

    [HttpGet("followers", Name = "GetUserFollowers")]
    [AllowAnonymous]
    public async Task<ActionResult<List<UserFollowDto>>> GetUserFollowers([FromRoute] Guid userId, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetUserFollowersQuery(userId), ct);
        return Ok(result);
    }

    [HttpGet("following", Name = "GetUserFollowing")]
    [AllowAnonymous]
    public async Task<ActionResult<List<UserFollowDto>>> GetUserFollowing([FromRoute] Guid userId, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetUserFollowingQuery(userId), ct);
        return Ok(result);
    }
}