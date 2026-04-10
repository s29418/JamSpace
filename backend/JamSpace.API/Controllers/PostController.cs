using JamSpace.API.Extensions;
using JamSpace.Application.Common.Models;
using JamSpace.Application.Features.Posts.DTOs;
using JamSpace.Application.Features.Posts.Queries.GetExploreFeed;
using JamSpace.Application.Features.Posts.Queries.GetFollowedFeed;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JamSpace.API.Controllers;

[ApiController]
[Route("api/posts")]
public class PostController : ControllerBase
{
    private readonly IMediator _mediator;

    public PostController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("explore")]
    [AllowAnonymous]
    public async Task<ActionResult<CursorResult<PostDto>>> GetExploreFeed([FromQuery] DateTimeOffset? before,
        [FromQuery] int take = 30, CancellationToken ct = default)
    {
        return Ok(await _mediator.Send(new GetExploreFeedQuery(before, take), ct));
    }

    [HttpGet("feed")]
    [Authorize]
    public async Task<ActionResult<CursorResult<PostDto>>> GetFollowedFeed([FromQuery] DateTimeOffset? before, 
        [FromQuery] int take = 30, CancellationToken ct = default)
    {
        var userId = User.GetUserId();
        var result = await _mediator.Send(new GetFollowedFeedQuery(userId, before, take), ct);
        return Ok(result);
    }
}