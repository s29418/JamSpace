using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Domain.Enums;
using MediatR;

namespace JamSpace.Application.Features.TeamMembers.Commands.LeaveTeam;

public class LeaveTeamHandler : IRequestHandler<LeaveTeamCommand, Unit>
{
    private readonly ITeamMemberRepository _repo;
    
    public LeaveTeamHandler(ITeamMemberRepository repo)
    {
        _repo = repo;
    }

    public async Task<Unit> Handle(LeaveTeamCommand request, CancellationToken ct)
    {
        var leaders = await _repo.GetLeadersAsync(request.TeamId, ct);

        if (await _repo.HasRequiredRoleAsync(request.TeamId, request.UserId, FunctionalRole.Leader, ct) && leaders.Count == 1)
            throw new ConflictException("Last team leader cannot leave the team. You can delete the team instead.");
        
        await _repo.DeleteTeamMemberAsync(request.TeamId, request.UserId, ct);
        
        return Unit.Value;
    }
}