using DefaultNamespace;
using JamSpace.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace JamSpace.Infrastructure.Repositories;

public class TeamRepository : ITeamRepository
{
    private readonly JamSpaceDbContext _db;
    
    public TeamRepository(JamSpaceDbContext db)
    {
        _db = db;
    }
    
    public async Task<Guid> CreateTeamAsync(Team team, Guid creatorUserId)
    {
        _db.Teams.Add(team);
        _db.TeamMembers.Add(new TeamMember
        {
            Team = team,
            UserId = creatorUserId
        });

        await _db.SaveChangesAsync();

        return team.Id;
    }


    public async Task<Team?> GetTeamByIdAsync(Guid id)
    {
        return await _db.Teams
            .Include(t => t.CreatedBy)
            .Include(t => t.Members)
            .ThenInclude(m => m.User)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public Task InviteUserAsync(Guid teamId, Guid invitedUserId)
    {
        throw new NotImplementedException();
    }
}