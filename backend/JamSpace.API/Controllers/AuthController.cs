using JamSpace.API.Extensions;
using JamSpace.API.Requests;
using JamSpace.API.Responses;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Common.Settings;
using JamSpace.Application.Features.Authentication.Commands.RefreshToken;
using JamSpace.Application.Features.Authentication.Commands.Register;
using JamSpace.Application.Features.Authentication.Dtos;
using JamSpace.Application.Features.Authentication.Queries.Login;
using JamSpace.Application.Features.Authentication.Queries.VerifyPassword;
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
    private const string RefreshTokenCookieName = "refreshToken";

    public AuthController(IMediator mediator, IOptions<JwtSettings> jwtOptions)
    {
        _mediator = mediator;
        _jwtOptions = jwtOptions;
    }

    private CookieOptions BuildRefreshTokenCookieOptions()
    {
        return new CookieOptions
        {
            HttpOnly = true,
            Secure = false,
            SameSite = SameSiteMode.Lax,
            Expires = DateTimeOffset.UtcNow.AddDays(_jwtOptions.Value.RefreshDays),
            Path = "/"
        };
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginDto dto)
    {
        var userAgent = Request.Headers["User-Agent"].ToString();
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();

        var result = await _mediator.Send(new LoginUserQuery(dto.Email, dto.Password, userAgent, ip));
        
        Response.Cookies.Append(RefreshTokenCookieName, result.RefreshToken, BuildRefreshTokenCookieOptions());
        
        return Ok(new AuthResponse(result.UserId, result.UserName, result.Email, result.AccessToken));
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResultDto>> Register([FromBody] RegisterUserCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }
    
    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Refresh()
    {
        Request.Cookies.TryGetValue(RefreshTokenCookieName, out var cookie);
        var result = await _mediator.Send(new RefreshCommand(cookie ?? string.Empty));

        Response.Cookies.Append(RefreshTokenCookieName, result.RefreshToken, BuildRefreshTokenCookieOptions());

        return Ok(new AuthResponse(result.UserId, result.UserName, result.Email, result.AccessToken));
    }

    [Authorize]
    [HttpPost("verify-password")]
    public async Task<IActionResult> VerifyPassword([FromBody] string password)
    {
        var userId = User.GetUserId();
        await _mediator.Send(new VerifyPasswordQuery(userId, password));
        return NoContent();
    }
    
    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(
        [FromServices] IAuthSessionService sessions,
        CancellationToken ct)
    {
        Request.Cookies.TryGetValue(RefreshTokenCookieName, out var cookie);
        var userId = User.GetUserId();

        await sessions.LogoutAsync(userId, cookie, ct);

        Response.Cookies.Delete(RefreshTokenCookieName, BuildRefreshTokenCookieOptions());
        return NoContent();
    }

    [Authorize]
    [HttpPost("logout-all")]
    public async Task<IActionResult> LogoutAll(
        [FromServices] IAuthSessionService sessions,
        CancellationToken ct)
    {
        var userId = User.GetUserId();
        await sessions.LogoutEverywhereAsync(userId, ct);

        Response.Cookies.Delete(RefreshTokenCookieName, BuildRefreshTokenCookieOptions());
        return NoContent();
    }
}