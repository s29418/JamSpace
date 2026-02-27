using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Common.Persistence;
using JamSpace.Domain.Enums;
using MediatR;

namespace JamSpace.Application.Features.Teams.Commands.DeleteTeam;

public class DeleteTeamHandler : IRequestHandler<DeleteTeamCommand, Unit>
{
    private readonly ITeamRepository _teams;
    private readonly ITeamMemberRepository _teamMembers;
    private readonly IUnitOfWork _uow;

    public DeleteTeamHandler(ITeamRepository teams, ITeamMemberRepository teamMembers, IUnitOfWork uow)
    {
        _teams = teams;
        _teamMembers = teamMembers;
        _uow = uow;
    }

    public async Task<Unit> Handle(DeleteTeamCommand request, CancellationToken ct)
    {
        if (!await _teamMembers.HasRequiredRoleAsync(request.TeamId, request.RequestingUserId, FunctionalRole.Leader, ct))
            throw new ForbiddenAccessException("Only team leader can delete teams.");

        var team = await _teams.GetByIdAsync(request.TeamId, ct)
                   ?? throw new NotFoundException("Team not found.");

        _teams.Remove(team);

        await _uow.SaveChangesAsync(ct);

        return Unit.Value;
    }
}