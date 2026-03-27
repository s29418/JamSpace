using JamSpace.API.Extensions;
using JamSpace.Application.Features.UserSkills.Commands.AddUserSkill;
using JamSpace.Application.Features.UserSkills.Commands.DeleteUserSkill;
using JamSpace.Application.Features.UserSkills.DTOs;
using JamSpace.Application.Features.UserSkills.Queries.GetUserSkills;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JamSpace.API.Controllers;

[ApiController]
[Route("api/users/{userId:guid}/skills")]
public class UserSkillController : ControllerBase
{
    private readonly IMediator _mediator;
    
    public UserSkillController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<List<UserSkillDto>>> AddUserSkill(
        [FromRoute] Guid userId, [FromBody] string skillName, CancellationToken ct)
    {
        var authId = User.GetUserId();
        if (authId != userId) return Unauthorized("You can only modify your own skills.");
        var result = await _mediator.Send(new AddUserSkillCommand(userId, skillName), ct);
        return CreatedAtRoute("GetUserSkills", new { userId }, result);
    }

    [HttpGet(Name = "GetUserSkills")]
    [AllowAnonymous]
    public async Task<ActionResult<List<UserSkillDto>>> GetUserSkills([FromRoute] Guid userId, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetUserSkillsQuery(userId), ct);
        return Ok(result);
    }

    [HttpDelete("{skillId:guid}")]
    [Authorize]
    public async Task<IActionResult> DeleteUserSkill([FromRoute] Guid userId, Guid skillId, CancellationToken ct)
    {
        var authId = User.GetUserId();
        if (authId != userId) return Unauthorized("You can only modify your own genres.");
        await _mediator.Send(new DeleteUserSkillCommand(userId, skillId), ct);
        return NoContent();
    }
    
}