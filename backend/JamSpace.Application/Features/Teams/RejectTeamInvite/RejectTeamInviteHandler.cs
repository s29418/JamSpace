using JamSpace.Application.Interfaces;
using MediatR;

namespace JamSpace.Application.Features.Teams.RejectTeamInvite;

public class RejectTeamInviteHandler : IRequestHandler<RejectTeamInviteCommand, Unit>
{
    private readonly ITeamRepository _repo;

    public RejectTeamInviteHandler(ITeamRepository repo)
    {
        _repo = repo;
    }

    public async Task<Unit> Handle(RejectTeamInviteCommand request, CancellationToken cancellationToken)
    {
        await _repo.RejectInviteAsync(request.InviteId, request.UserId, cancellationToken);
        return Unit.Value;
    }
}
