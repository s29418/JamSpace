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
    
    [HttpPost("follow")]
    [Authorize]
    public async Task<ActionResult<BasicUserFollowDto>> FollowUser([FromRoute] Guid userId, CancellationToken ct)
    {
        var followerId = User.GetUserId();
        
        var result = await _mediator.Send(new FollowUserCommand(followerId, userId), ct);
        return CreatedAtAction(nameof(GetUserFollowing), new { userId = followerId }, result);
    }

    [HttpDelete("follow")]
    [Authorize]
    public async Task<IActionResult> UnfollowUser([FromRoute] Guid userId, CancellationToken ct)
    {
        var followerId = User.GetUserId();

        await _mediator.Send(new UnfollowUserCommand(followerId, userId), ct);
        return NoContent();
    }

    [HttpGet("followers", Name = "GetUserFollowers")]
    [AllowAnonymous]
    public async Task<ActionResult<List<DetailedUserFollowDto>>> GetUserFollowers([FromRoute] Guid userId, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetUserFollowersQuery(userId), ct);
        return Ok(result);
    }

    [HttpGet("following", Name = "GetUserFollowing")]
    [AllowAnonymous]
    public async Task<ActionResult<List<DetailedUserFollowDto>>> GetUserFollowing([FromRoute] Guid userId, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetUserFollowingQuery(userId), ct);
        return Ok(result);
    }
}