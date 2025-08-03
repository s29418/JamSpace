using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using MediatR;

namespace JamSpace.Application.Features.Teams.Commands.LeaveTeam;

public class LeaveTeamHandler : IRequestHandler<LeaveTeamCommand, Unit>
{
    private readonly ITeamRepository _repo;
    
    public LeaveTeamHandler(ITeamRepository repo)
    {
        _repo = repo;
    }

    public async Task<Unit> Handle(LeaveTeamCommand request, CancellationToken ct)
    {
        var leaders = await _repo.GetLeadersAsync(request.TeamId, ct);

        if (await _repo.IsUserALeaderAsync(request.TeamId, request.UserId) && leaders.Count == 1)
            throw new ConflictException("Last team leader cannot leave the team. You can delete the team instead.");
        
        await _repo.DeleteTeamMemberAsync(request.TeamId, request.UserId, ct);
        
        return Unit.Value;
    }
}