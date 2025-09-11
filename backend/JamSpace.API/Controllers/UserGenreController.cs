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
[Route("api/users/{userId:guid}/genres")]
public class UserGenreController : ControllerBase
{
    private readonly IMediator _mediator;
    
    public UserGenreController(IMediator mediator) => _mediator = mediator;
    
    [HttpPost]
    // [Authorize]
    public async Task<ActionResult<UserGenreDto>> AddUserGenre([FromRoute] Guid userId, [FromBody] string genreName, CancellationToken ct)
    {
        var authId = User.GetUserId();
        if (authId != userId) return Unauthorized("You can only modify your own genres.");
        var result = await _mediator.Send(new AddUserGenreCommand(userId, genreName), ct);
        return CreatedAtRoute("GetUserGenres", new { userId = userId }, result);
    }
    
    [HttpGet(Name = "GetUserGenres")]
    [AllowAnonymous]
    public async Task<ActionResult<List<UserGenreDto>>> GetUserGenres([FromRoute] Guid userId, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetUserGenresQuery(userId), ct);
        return Ok(result);
    }

    [HttpDelete("{genreId:guid}")]
    // [Authorize]
    public async Task<IActionResult> DeleteUserGenre([FromRoute] Guid userId, Guid genreId, CancellationToken ct)
    {
        var authId = User.GetUserId();
        if (authId != userId) return Unauthorized("You can only modify your own genres.");
        await _mediator.Send(new DeleteUserGenreCommand(userId, genreId), ct);
        return NoContent();
    }
}