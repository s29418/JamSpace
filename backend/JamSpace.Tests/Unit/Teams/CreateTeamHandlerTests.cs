using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Common.Persistence;
using JamSpace.Application.Features.TeamMembers.Commands.LeaveTeam;
using JamSpace.Domain.Enums;
using MediatR;

namespace JamSpace.Tests.Unit.Teams;

public sealed class LeaveTeamHandler : IRequestHandler<LeaveTeamCommand, MediatR.Unit>
{
    private readonly ITeamMemberRepository _repo;
    private readonly IUnitOfWork _uow;

    public LeaveTeamHandler(ITeamMemberRepository repo, IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public async Task<MediatR.Unit> Handle(LeaveTeamCommand request, CancellationToken ct)
    {
        var leaders = await _repo.GetLeadersAsync(request.TeamId, ct);

        if (await _repo.HasRequiredRoleAsync(request.TeamId, request.UserId, FunctionalRole.Leader, ct) && leaders.Count == 1)
            throw new ConflictException("Last team leader cannot leave the team. You can delete the team instead.");

        var member = await _repo.GetByTeamAndUserAsync(request.TeamId, request.UserId, ct)
                     ?? throw new NotFoundException("Team member not found.");

        _repo.Remove(member);
        await _uow.SaveChangesAsync(ct);

        return MediatR.Unit.Value;
    }
}