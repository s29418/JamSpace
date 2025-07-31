using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using MediatR;

namespace JamSpace.Application.Features.Teams.Commands.CancelTeamInvite;

public class CancelTeamInviteHandler : IRequestHandler<CancelTeamInviteCommand, Unit>
{
    private readonly ITeamRepository _repo;
    
    public CancelTeamInviteHandler(ITeamRepository repo)
    {
        _repo = repo;
    }

    public async Task<Unit> Handle(CancelTeamInviteCommand request, CancellationToken ct)
    {
        var teamInvite = await _repo.GetTeamInviteByIdAsync(request.TeamInviteId, ct);
        
        if (!await _repo.IsUserInTeamAsync(teamInvite.TeamId,request.RequestingUserId))
            throw new ForbiddenAccessException("You are not a member of this team.");

        await _repo.CancelTeamInviteAsync(request.TeamInviteId, request.RequestingUserId, ct);
        return Unit.Value;
    }
}