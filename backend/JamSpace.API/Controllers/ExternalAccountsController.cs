using JamSpace.API.Extensions;
using JamSpace.Application.Common.Models;
using JamSpace.Application.Features.ExternalAccounts.Commands.CompleteConnection;
using JamSpace.Application.Features.ExternalAccounts.Commands.Disconnect;
using JamSpace.Application.Features.ExternalAccounts.Commands.StartConnection;
using JamSpace.Application.Features.ExternalAccounts.DTOs;
using JamSpace.Application.Features.ExternalAccounts.Queries.GetMyExternalAccounts;
using JamSpace.Application.Features.ExternalAccounts.Queries.GetMySpotifyPlaylists;
using JamSpace.Application.Features.ExternalAccounts.Queries.GetUserPublicExternalAccounts;
using JamSpace.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JamSpace.API.Controllers;

[ApiController]
[Route("api/me/external-accounts")]
[Authorize]
public class ExternalAccountsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ExternalAccountsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("/api/users/{userId:guid}/external-accounts")]
    [AllowAnonymous]
    public async Task<ActionResult<IReadOnlyList<PublicUserExternalAccountDto>>> GetUserExternalAccounts(
        [FromRoute] Guid userId,
        CancellationToken ct)
    {
        var result = await _mediator.Send(new GetUserPublicExternalAccountsQuery(userId), ct);
        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<UserExternalAccountDto>>> GetMyExternalAccounts(
        CancellationToken ct)
    {
        var userId = User.GetUserId();
        var result = await _mediator.Send(new GetMyExternalAccountsQuery(userId), ct);
        return Ok(result);
    }

    [HttpGet("spotify/playlists")]
    public async Task<ActionResult<IReadOnlyList<SpotifyPlaylistDto>>> GetMySpotifyPlaylists(
        CancellationToken ct)
    {
        var userId = User.GetUserId();
        var result = await _mediator.Send(new GetMySpotifyPlaylistsQuery(userId), ct);
        return Ok(result);
    }

    [HttpPost("{provider}/connect")]
    public async Task<ActionResult<ExternalAuthUrl>> Connect(
        [FromRoute] string provider,
        [FromQuery] string? returnUrl,
        CancellationToken ct)
    {
        if (!Enum.TryParse<ExternalMusicProvider>(provider, ignoreCase: true, out var parsedProvider))
            return BadRequest(new { message = "Unsupported external account provider." });

        var userId = User.GetUserId();
        var result = await _mediator.Send(
            new StartExternalAccountConnectionCommand(userId, parsedProvider, returnUrl),
            ct);

        return Ok(result);
    }

    [HttpGet("/api/external-accounts/{provider}/callback")]
    [AllowAnonymous]
    public async Task<IActionResult> Callback(
        [FromRoute] string provider,
        [FromQuery] string? code,
        [FromQuery] string? state,
        [FromQuery] string? error,
        CancellationToken ct)
    {
        if (!Enum.TryParse<ExternalMusicProvider>(provider, ignoreCase: true, out var parsedProvider))
            return BadRequest(new { message = "Unsupported external account provider." });

        if (!string.IsNullOrWhiteSpace(error))
            return BadRequest(new { message = "OAuth authorization failed.", error });

        if (string.IsNullOrWhiteSpace(code))
            return BadRequest(new { message = "OAuth code is required." });

        if (string.IsNullOrWhiteSpace(state))
            return BadRequest(new { message = "OAuth state is required." });

        var result = await _mediator.Send(
            new CompleteExternalAccountConnectionCommand(parsedProvider, state, code),
            ct);

        if (string.IsNullOrWhiteSpace(result.ReturnUrl))
            return Ok(result.Account);

        return Redirect(BuildOAuthRedirectUrl(result.ReturnUrl, parsedProvider, true));
    }

    [HttpDelete("{provider}")]
    public async Task<IActionResult> Disconnect([FromRoute] string provider, CancellationToken ct)
    {
        if (!Enum.TryParse<ExternalMusicProvider>(provider, ignoreCase: true, out var parsedProvider))
            return BadRequest(new { message = "Unsupported external account provider." });

        var userId = User.GetUserId();
        await _mediator.Send(new DisconnectExternalAccountCommand(userId, parsedProvider), ct);
        return NoContent();
    }

    private static string BuildOAuthRedirectUrl(string returnUrl, ExternalMusicProvider provider, bool success)
    {
        var separator = returnUrl.Contains('?') ? '&' : '?';
        return $"{returnUrl}{separator}externalAccountProvider={provider}&externalAccountConnected={success.ToString().ToLowerInvariant()}";
    }
}
