using JamSpace.API.Extensions;
using JamSpace.API.Requests;
using JamSpace.Application.Common.Models;
using JamSpace.Application.Features.Posts.Commands.CommentPost;
using JamSpace.Application.Features.Posts.Commands.Create;
using JamSpace.Application.Features.Posts.Commands.Delete;
using JamSpace.Application.Features.Posts.Commands.DeleteComment;
using JamSpace.Application.Features.Posts.Commands.DeleteRepost;
using JamSpace.Application.Features.Posts.Commands.Like;
using JamSpace.Application.Features.Posts.Commands.Repost;
using JamSpace.Application.Features.Posts.Commands.Unlike;
using JamSpace.Application.Features.Posts.DTOs;
using JamSpace.Application.Features.Posts.Queries.GetExploreFeed;
using JamSpace.Application.Features.Posts.Queries.GetFollowedFeed;
using JamSpace.Application.Features.Posts.Queries.GetPostById;
using JamSpace.Application.Features.Posts.Queries.GetPostComment;
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

    // POSTS
    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<ActionResult<PostDto>> GetById([FromRoute] Guid id, CancellationToken ct)
    {
        Guid? userId = User.TryGetUserId();
        return Ok(await _mediator.Send(new GetPostByIdQuery(id, userId), ct));
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
    public async Task<ActionResult<PostDto>> CreatePost([FromForm] CreatePostRequest request, 
        CancellationToken ct = default)
    {
        var userId = User.GetUserId();

        var result = await _mediator.Send(new CreatePostCommand
            (userId, request.Content, request.File?.ToFileUpload()), ct);

        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpDelete("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> DeletePost([FromRoute] Guid id, CancellationToken ct = default)
    {
        var userId = User.GetUserId();
        await _mediator.Send(new DeletePostCommand(id, userId), ct);
        return NoContent();
    }

    
    // REPOSTS
    [HttpPost("{id:guid}/repost")]
    [Authorize]
    public async Task<ActionResult<PostDto>> Repost([FromRoute] Guid id, CancellationToken ct = default)
    {
        var userId = User.GetUserId();
        var result = await _mediator.Send(new RepostPostCommand(userId, id), ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpDelete("{id:guid}/repost")]
    [Authorize]
    public async Task<IActionResult> DeleteRepost([FromRoute] Guid id, CancellationToken ct = default)
    {
        var userId = User.GetUserId();
        await _mediator.Send(new DeleteRepostCommand(userId, id), ct);
        return NoContent();
    }
    
    
    // LIKES
    [HttpPost("{id:guid}/likes")]
    [Authorize]
    public async Task<IActionResult> LikePost([FromRoute] Guid id, CancellationToken ct = default)
    {
        var userId = User.GetUserId();
        await _mediator.Send(new LikePostCommand(userId, id), ct);
        return NoContent();
    }
    
    [HttpDelete("{id:guid}/likes")]
    [Authorize]
    public async Task<IActionResult> UnlikePost([FromRoute] Guid id, CancellationToken ct = default)
    {
        var userId = User.GetUserId();
        await _mediator.Send(new UnlikePostCommand(userId, id), ct);
        return NoContent();
    }
    
    
    // COMMENTS
    [HttpGet("comments/{commentId:guid}")]
    [AllowAnonymous]
    public async Task<ActionResult<PostCommentDto>> GetPostComment([FromRoute] Guid commentId, CancellationToken ct = default)
    {
        await _mediator.Send(new GetPostCommentQuery(commentId), ct);
        return NoContent();
    }

    [HttpPost("{id:guid}/comments")]
    [Authorize]
    public async Task<ActionResult<PostCommentDto>> CreatePostComment([FromRoute] Guid id, [FromBody] string content,
        CancellationToken ct = default)
    {
        var userId = User.GetUserId();
        var result = await _mediator.Send(new CommentPostCommand(userId, id, content), ct);
        return CreatedAtAction(nameof(GetPostComment), new { commentId = result.Id }, result);
    }

    [HttpDelete("comments/{commentId:guid}")]
    [Authorize]
    public async Task<IActionResult> DeletePostComment([FromRoute] Guid commentId, CancellationToken ct = default)
    {
        var userId = User.GetUserId();
        await _mediator.Send(new DeleteCommentCommand(commentId, userId), ct);
        return NoContent();
    }
}