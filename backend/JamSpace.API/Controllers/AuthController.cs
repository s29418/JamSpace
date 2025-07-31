using DefaultNamespace;
using JamSpace.Application.Common.Authentication;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace JamSpace.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResultDto>> Login([FromBody] LoginUserQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result); 
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResultDto>> Register([FromBody] RegisterUserCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}