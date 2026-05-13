using JamSpace.API.Extensions;
using JamSpace.Application.Common.Models;
using JamSpace.Application.Features.ExternalAccounts.Commands.CompleteConnection;
using JamSpace.Application.Features.ExternalAccounts.Commands.StartConnection;
using JamSpace.Application.Features.ExternalAccounts.DTOs;
using JamSpace.Application.Features.ExternalAccounts.Queries.GetMyExternalAccounts;
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

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<UserExternalAccountDto>>> GetMyExternalAccounts(
        CancellationToken ct)
    {
        var userId = User.GetUserId();
        var result = await _mediator.Send(new GetMyExternalAccountsQuery(userId), ct);
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

    [HttpGet("{provider}/callback")]
    public async Task<ActionResult<UserExternalAccountDto>> Callback(
        [FromRoute] string provider,
        [FromQuery] string? code,
        [FromQuery] string? state,
        CancellationToken ct)
    {
        if (!Enum.TryParse<ExternalMusicProvider>(provider, ignoreCase: true, out var parsedProvider))
            return BadRequest(new { message = "Unsupported external account provider." });

        if (string.IsNullOrWhiteSpace(code))
            return BadRequest(new { message = "OAuth code is required." });

        if (string.IsNullOrWhiteSpace(state))
            return BadRequest(new { message = "OAuth state is required." });

        var result = await _mediator.Send(
            new CompleteExternalAccountConnectionCommand(parsedProvider, state, code),
            ct);

        return Ok(result);
    }
}
