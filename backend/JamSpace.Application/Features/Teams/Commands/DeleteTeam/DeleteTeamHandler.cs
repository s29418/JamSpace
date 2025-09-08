using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using MediatR;

namespace JamSpace.Application.Features.Teams.Commands.DeleteTeam;

public class DeleteTeamHandler : IRequestHandler<DeleteTeamCommand, Unit>
{
    private readonly ITeamRepository _teamRepository;
    private readonly ITeamMemberRepository _teamMemberRepository;

    public DeleteTeamHandler(ITeamRepository teamRepository, ITeamMemberRepository teamMemberRepository)
    {
        _teamRepository = teamRepository;
        _teamMemberRepository = teamMemberRepository;
    }

    public async Task<Unit> Handle(DeleteTeamCommand request, CancellationToken ct)
    {
        if (!await _teamMemberRepository.IsUserALeaderAsync(request.TeamId, request.RequestingUserId, ct))
            throw new ForbiddenAccessException("Only team leader can delete teams.");
        
        await _teamRepository.DeleteTeamAsync(request.TeamId, ct);
        return Unit.Value;
    }
}