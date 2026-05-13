using JamSpace.API.Extensions;
using JamSpace.Application.Common.Models;
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
}
