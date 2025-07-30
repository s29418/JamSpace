using JamSpace.Application.Features.Teams.Dtos;
using JamSpace.Application.Interfaces;
using MediatR;

namespace JamSpace.Application.Features.Teams.Commands.ChangeTeamMemberFunctionalRole;

public class ChangeTeamMemberFunctionalRoleHandler : IRequestHandler<ChangeTeamMemberFunctionalRoleCommand, Unit>
{
    private readonly ITeamRepository _repo;
    
    public ChangeTeamMemberFunctionalRoleHandler(ITeamRepository repo)
    {
        _repo = repo;
    }

    public async Task<Unit> Handle(ChangeTeamMemberFunctionalRoleCommand request, CancellationToken cancellationToken)
    {
        await _repo.ChangeTeamMemberFunctionalRoleAsync(request.TeamId, request.RequestingUserId, request.UserId, request.NewRole, cancellationToken);
        return Unit.Value;
    }
}