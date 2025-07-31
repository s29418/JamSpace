using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using MediatR;

namespace JamSpace.Application.Features.Teams.Commands.DeleteTeam;

public class DeleteTeamHandler : IRequestHandler<DeleteTeamCommand, Unit>
{
    private readonly ITeamRepository _repo;

    public DeleteTeamHandler(ITeamRepository repo)
    {
        _repo = repo;
    }

    public async Task<Unit> Handle(DeleteTeamCommand request, CancellationToken cancellationToken)
    {
        if (!await _repo.IsUserALeaderAsync(request.TeamId, request.RequestingUserId))
            throw new ForbiddenAccessException("Only team leader can delete teams.");
        
        await _repo.DeleteTeamAsync(request.TeamId, cancellationToken);
        return Unit.Value;
    }
}