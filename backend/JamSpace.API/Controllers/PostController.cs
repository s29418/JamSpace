using JamSpace.API.Extensions;
using JamSpace.API.Requests;
using JamSpace.Application.Common.Models;
using JamSpace.Application.Features.Posts.Commands.CreatePost;
using JamSpace.Application.Features.Posts.DTOs;
using JamSpace.Application.Features.Posts.Queries.GetExploreFeed;
using JamSpace.Application.Features.Posts.Queries.GetFollowedFeed;
using JamSpace.Application.Features.Posts.Queries.GetPostById;
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

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<ActionResult<PostDto>> GetById([FromRoute] Guid id, CancellationToken ct)
    {
        return Ok(await _mediator.Send(new GetPostByIdQuery(id), ct));
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

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<PostDto>> CreatePost([FromForm] CreatePostRequest request, CancellationToken ct = default)
    {
        var userId = User.GetUserId();

        var result = await _mediator.Send(new CreatePostCommand
            (userId, request.Content, request.File?.ToFileUpload()), ct);

        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }
}