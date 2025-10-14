using JamSpace.API.Extensions;
using JamSpace.Application.Features.Users.DTOs;
using JamSpace.Application.Features.Users.Queries.GetDetails;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JamSpace.API.Controllers;

[ApiController]
[Route("api/users")]
public class UserController : ControllerBase
{
    private readonly IMediator _mediator;
    
    public UserController(IMediator mediator) => _mediator = mediator;
    
    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<UserDto>> GetMe(CancellationToken ct)
    {
        var userId = User.GetUserId();
        var dto = await _mediator.Send(new GetUserByIdQuery(userId), ct);

        
        return Ok(dto);
    }
    
    [HttpPut]
    [Authorize]
    public async Task<ActionResult<UserDto>> UpdateUserProfile([FromBody] UpdateUserProfileCommand command, CancellationToken ct)
    {
        if (command.UserId == Guid.Empty)
            return BadRequest("UserId is required.");
        
        var result = await _mediator.Send(command, ct);
        return Ok(result);
    }
}