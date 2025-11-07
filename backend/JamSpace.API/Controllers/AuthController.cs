using JamSpace.API.Requests;
using JamSpace.API.Responses;
using JamSpace.Application.Common.Settings;
using JamSpace.Application.Features.Authentication.Dtos;
using JamSpace.Application.Features.Authentication.Login;
using JamSpace.Application.Features.Authentication.Register;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace JamSpace.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IOptions<JwtSettings> _jwtOptions;

    public AuthController(IMediator mediator, IOptions<JwtSettings> jwtOptions)
    {
        _mediator = mediator;
        _jwtOptions = jwtOptions;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginDto dto)
    {
        var userAgent = Request.Headers["User-Agent"].ToString();
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();

        var result = await _mediator.Send(new LoginUserQuery(dto.Email, dto.Password, userAgent, ip));
        
        Response.Cookies.Append("refreshToken", result.RefreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = DateTimeOffset.UtcNow.AddDays(_jwtOptions.Value.RefreshDays)
        });
        
        return Ok(new AuthResponse(result.UserId, result.UserName, result.Email, result.AccessToken));
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResultDto>> Register([FromBody] RegisterUserCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}