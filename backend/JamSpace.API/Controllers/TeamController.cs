// using DefaultNamespace;
// using JamSpace.Application.Services;
// using Microsoft.AspNetCore.Mvc;
//
// namespace JamSpace.API.Controllers;
//
// [ApiController]
// [Route("api/[controller]")]
// public class TeamController : ControllerBase
// {
//     private readonly TeamService _teamService;
//     
//     public TeamController(TeamService teamService)
//     {
//         _teamService = teamService;
//     }
//
//     [HttpPost]
//     public async Task<IActionResult> CreateTeam([FromBody] CreateTeamDto dto)
//     {
//         var team = await _teamService.CreateTeam(dto);
//         return Ok(team);
//     }
//
// }