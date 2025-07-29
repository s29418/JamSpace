using JamSpace.Application.Features.Uploads.UploadTeamPicture;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JamSpace.API.Controllers;

[ApiController]
[Route("api/uploads")]
public class UploadController : ControllerBase
{
    private readonly IMediator _mediator;

    public UploadController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("team-picture")]
    [Authorize]
    public async Task<IActionResult> UploadTeamPicture([FromForm] UploadTeamPictureRequest request)
    {
        if (request.File.Length == 0)
            return BadRequest("No file provided.");

        var url = await _mediator.Send(new UploadTeamPictureCommand(request.File));
        return Ok(new { url });
    }
}
