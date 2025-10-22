using JamSpace.API.Extensions;
using JamSpace.API.Requests;
using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Features.Users.Commands.ChangePassword;
using JamSpace.Application.Features.Users.Commands.Delete;
using JamSpace.Application.Features.Users.Commands.UpdateUserProfile;
using JamSpace.Application.Features.Users.DTOs;
using JamSpace.Application.Features.Users.Queries.GetDetails;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JamSpace.API.Controllers;

[ApiController]
[Route("api/users")]
public class UserController : ControllerBase
{
    private readonly IMediator _mediator;
    
    public UserController(IMediator mediator) => _mediator = mediator;
    
    [HttpGet(("{id:guid}"))]
    [AllowAnonymous]
    public async Task<ActionResult<UserDto>> GetDetails([FromRoute] Guid id, CancellationToken ct)
    {
        var requestingUserId = User.GetUserId();
        var dto = await _mediator.Send(new GetUserByIdQuery(id, requestingUserId), ct);
        return Ok(dto);
    }
    
    [HttpPatch("{id:guid}")]
    [Authorize]
    public async Task<ActionResult<UserDto>> UpdateUserProfile([FromRoute] Guid id
    , [FromBody] UpdateUserProfileRequest request, CancellationToken ct)
    {
        if (id != User.GetUserId())
            throw new NotFoundException("You can only modify your own profile.");

        var result = await _mediator.Send(new UpdateUserProfileCommand(
            id, request.SetBio, request.Bio, request.SetProfilePicture, request.ProfilePictureUrl, 
            request.SetLocation, request.Location, request.SetEmail, request.Email
            ), ct);
        return Ok(result);
    }

    [HttpPatch("{id:guid}/password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromRoute] Guid id, [FromBody] ChangePasswordRequest request, CancellationToken ct)
    {
        if (id != User.GetUserId())
            throw new NotFoundException("You can only modify your own password.");
        
        await _mediator.Send(new ChangePasswordCommand(id, request.CurrentPassword, request.NewPassword), ct);
        return NoContent();
    }

    [HttpDelete]
    [Authorize]
    public async Task<IActionResult> DeleteUser([FromRoute] Guid id, CancellationToken ct)
    {
        if (id != User.GetUserId())
            throw new NotFoundException("You can only modify your own profile.");
        
        await _mediator.Send(new DeleteUserCommand(id), ct);
        return NoContent(); 
    }
}