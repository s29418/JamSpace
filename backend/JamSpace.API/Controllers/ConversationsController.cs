using JamSpace.API.Extensions;
using JamSpace.Application.Features.Conversations.DTOs;
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
}