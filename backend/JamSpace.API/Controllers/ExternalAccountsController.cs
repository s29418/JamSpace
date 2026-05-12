using JamSpace.API.Extensions;
using JamSpace.Application.Features.ExternalAccounts.DTOs;
using JamSpace.Application.Features.ExternalAccounts.Queries.GetMyExternalAccounts;
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
}
