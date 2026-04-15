using JamSpace.Application.Features.Media.Queries.GetMedia;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JamSpace.API.Controllers;

[ApiController]
[Route("api/media")]
public class MediaController : ControllerBase
{
    private readonly IMediator _mediator;

    public MediaController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetMedia([FromQuery] string url, CancellationToken ct = default)
    {
        var file = await _mediator.Send(new GetMediaQuery(url), ct);

        if (file is null)
            return NotFound();

        return File(file.Content, file.ContentType, file.FileName, enableRangeProcessing: true);
    }
}
