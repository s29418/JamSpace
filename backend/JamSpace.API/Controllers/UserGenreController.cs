using JamSpace.API.Extensions;
using JamSpace.Application.Features.UserGenres.Commands.AddUserGenre;
using JamSpace.Application.Features.UserGenres.Commands.RemoveUserGenre;
using JamSpace.Application.Features.UserGenres.DTOs;
using JamSpace.Application.Features.UserGenres.Queries.GetUsersGenres;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JamSpace.API.Controllers;

[ApiController]
[Route("api/users")]
public class UserGenreController : ControllerBase
{
    private readonly IMediator _mediator;
    
    public UserGenreController(IMediator mediator) => _mediator = mediator;
    
    [HttpPost("{userId}/genres")]
    // [Authorize]
    public async Task<ActionResult<UserGenreDto>> AddUserGenre([FromBody] string genreName, CancellationToken ct)
    {
        var userId = User.GetUserId();
        var result = await _mediator.Send(new AddUserGenreCommand(userId, genreName), ct);
        return CreatedAtRoute("GetUserGenres", new { userId = userId }, result);
        
    }
    
    [HttpGet("{userId}/genres")]
    // [Authorize]
    public async Task<ActionResult<List<UserGenreDto>>> GetUserGenres(CancellationToken ct)
    {
        var userId = User.GetUserId();
        var result = await _mediator.Send(new GetUserGenresQuery(userId), ct);
        return Ok(result);
    }

    [HttpDelete("{userId}/genres/{genreId}")]
    // [Authorize]
    public async Task<IActionResult> DeleteUserGenre(Guid genreId, CancellationToken ct)
    {
        var userId = User.GetUserId();
        await _mediator.Send(new DeleteUserGenreCommand(userId, genreId), ct);
        return NoContent();
    }
}