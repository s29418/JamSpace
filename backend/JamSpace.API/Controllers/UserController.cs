using JamSpace.API.Extensions;
using JamSpace.API.Requests;
using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Features.Users.Commands.ChangePassword;
using JamSpace.Application.Features.Users.Commands.Delete;
using JamSpace.Application.Features.Users.Commands.UpdateUserProfile;
using JamSpace.Application.Features.Users.Commands.UpdateUserProfilePicture;
using JamSpace.Application.Features.Users.DTOs;
using JamSpace.Application.Features.Users.Queries.GetDetails;
using JamSpace.Application.Features.Users.Queries.SearchUsers;
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
    
    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<ActionResult<UserDto>> GetDetails([FromRoute] Guid id, CancellationToken ct)
    {
        Guid? requestingUserId = null;

        try
        {
            requestingUserId = User.GetUserId();
        }
        catch (UnauthorizedAccessException)
        {
        }
        
        var dto = await _mediator.Send(new GetUserByIdQuery(id, requestingUserId ?? Guid.Empty), ct);
        return Ok(dto);
    }
    

    [HttpPatch("{id:guid}")]
    [Authorize]
    public async Task<ActionResult<UserDto>> UpdateUserProfile(
        [FromRoute] Guid id,
        [FromForm] UpdateUserProfileRequest request,
        CancellationToken ct)
    {
        var currentUserId = User.GetUserId();

        if (id != currentUserId)
            throw new NotFoundException("You can only modify your own profile.");

        UserDto? pictureUpdateResult = null;

        if (request.SetProfilePicture)
        {
            if (request.File is null)
                return BadRequest("Profile picture file is required.");

            pictureUpdateResult = await _mediator.Send(
                new UpdateUserProfilePictureCommand(
                    id,
                    currentUserId,
                    request.File.ToFileUpload()
                ),
                ct);

            request.SetProfilePicture = false;
        }

        var hasNonPictureUpdates =
            request.SetDisplayName ||
            request.SetBio ||
            request.SetLocation ||
            request.SetEmail;

        if (!hasNonPictureUpdates)
            return Ok(pictureUpdateResult);

        var result = await _mediator.Send(new UpdateUserProfileCommand(
            id,
            request.SetDisplayName, request.DisplayName,
            request.SetBio, request.Bio,
            false, null,
            request.SetLocation, request.Location,
            request.SetEmail, request.Email
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

    [HttpDelete("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> DeleteUser([FromRoute] Guid id, CancellationToken ct)
    {
        if (id != User.GetUserId())
            throw new NotFoundException("You can only modify your own profile.");
        
        await _mediator.Send(new DeleteUserCommand(id), ct);
        return NoContent(); 
    }
    
    [AllowAnonymous]
    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] SearchUsersRequest request, CancellationToken ct)
    {
        var skills = SplitMulti(request.Skills);
        var genres = SplitMulti(request.Genres);

        Guid? currentUserId = User.GetUserId();

        var query = new SearchUsersQuery(
            Q: request.Q,
            Location: request.Location,
            Skills: skills,
            Genres: genres,
            Page: request.Page,
            PageSize: request.PageSize,
            CurrentUserId: currentUserId
        );

        var result = await _mediator.Send(query, ct);
        return Ok(result);
    }
    
    private static IReadOnlyList<string> SplitMulti(string[]? raw)
    {
        if (raw is null || raw.Length == 0)
            return Array.Empty<string>();

        return raw
            .SelectMany(v => (v)
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            .Where(v => !string.IsNullOrWhiteSpace(v))
            .ToArray();
    }
}