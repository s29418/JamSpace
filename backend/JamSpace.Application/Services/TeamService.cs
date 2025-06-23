// using DefaultNamespace;
//
// namespace JamSpace.Application.Services;
//
// public class TeamService
// {
//     private readonly JamSpaceDbContext _context;
//
//     public TeamService(JamSpaceDbContext context)
//     {
//         _context = context;
//     }
//
//     public async Task<Team> CreateTeam(CreateTeamDto dto)
//     {
//         var team = new Team
//         {
//             Id = Guid.NewGuid(),
//             Name = dto.Name,
//             TeamPictureUrl = dto.TeamPictureUrl,
//             CreatedById = dto.CreatorUserId
//         };
//
//         var member = new TeamMember
//         {
//             Id = Guid.NewGuid(),
//             TeamId = team.Id,
//             UserId = dto.CreatorUserId,
//             Role = "Owner"
//         };
//
//         _context.Teams.Add(team);
//         _context.TeamMembers.Add(member);
//         await _context.SaveChangesAsync();
//         
//         return team;
//     }
//     
// }