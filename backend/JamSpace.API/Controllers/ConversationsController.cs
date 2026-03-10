using JamSpace.API.Extensions;
using JamSpace.API.Responses;
using JamSpace.Application.Common.Models;
using JamSpace.Application.Features.Conversations.Commands.GetOrCreateDirectConversation;
using JamSpace.Application.Features.Conversations.Commands.MarkConversationAsRead;
using JamSpace.Application.Features.Conversations.DTOs;
using JamSpace.Application.Features.Conversations.Queries.GetConversationDetails;
using JamSpace.Application.Features.Conversations.Queries.GetConversationMessages;
using JamSpace.Application.Features.Conversations.Queries.GetMyConversations;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JamSpace.API.Controllers;

[ApiController]
[Route("api/conversations")]
public class ConversationsController : ControllerBase
{
    private readonly IMediator _mediator;
    public ConversationsController(IMediator mediator) => _mediator = mediator;

    
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<ConversationCardDto>>> GetConversations(CancellationToken ct)
    {
        var userId = User.GetUserId();
        var result = await _mediator.Send(new GetMyConversationsQuery(userId), ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [Authorize]
    public async Task<ActionResult<ConversationDetailsDto>> GetConversationDetails(Guid id, CancellationToken ct)
    {
        var userId = User.GetUserId();
        var result = await _mediator.Send(new GetConversationDetailsQuery(id, userId), ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}/messages")]
    [Authorize]
    public async Task<ActionResult<CursorResult<MessageDto>>> GetConversationMessages(Guid id,
        [FromQuery] DateTimeOffset? before, [FromQuery] int take = 50, CancellationToken ct = default)
    {
        var userId = User.GetUserId();
        var result = await _mediator.Send(new GetConversationMessagesQuery(id, userId, before, take), ct);
        return Ok(result);
    }

    [HttpPost("direct")]
    [Authorize]
    public async Task<ActionResult<ConversationIdResponse>> GetOrCreateDirectConversation([FromBody] GetOrCreateDirectConversationRequest otherUserId,
        CancellationToken ct)
    {
        var requestingUserId = User.GetUserId();
        var result = await _mediator
            .Send(new GetOrCreateDirectConversationCommand(requestingUserId, otherUserId.OtherUserId), ct);
        return Ok(new ConversationIdResponse{ ConversationId = result});
    }

    [HttpPost("{id:guid}/read")]
    [Authorize]
    public async Task<IActionResult> MarkAsRead(Guid id, CancellationToken ct)
    {
        var userId = User.GetUserId();
        await _mediator.Send(new MarkConversationAsReadCommand(id, userId, null), ct);
        return NoContent();
    }
}