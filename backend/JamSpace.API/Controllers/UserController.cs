using JamSpace.Application.Features.Users.DTOs;
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