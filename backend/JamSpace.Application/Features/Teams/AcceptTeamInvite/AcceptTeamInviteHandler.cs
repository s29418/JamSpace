using JamSpace.Application.Interfaces;
using MediatR;

namespace JamSpace.Application.Features.Teams.AcceptTeamInvite;

public class AcceptTeamInviteHandler : IRequestHandler<AcceptTeamInviteCommand>
{
    private readonly ITeamRepository _repo;

    public AcceptTeamInviteHandler(ITeamRepository repo)
    {
        _repo = repo;
    }

    public async Task<Unit> Handle(AcceptTeamInviteCommand request, CancellationToken cancellationToken)
    {
        await _repo.AcceptInviteAsync(request.InviteId, request.UserId, cancellationToken);
        return Unit.Value;
    }
}
