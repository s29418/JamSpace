using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Interfaces;
using MediatR;

namespace JamSpace.Application.Features.Teams.SendTeamInvite;

public class SendTeamInviteHandler : IRequestHandler<SendTeamInviteCommand>
{
    private readonly ITeamRepository _repo;

    public SendTeamInviteHandler(ITeamRepository repo)
    {
        _repo = repo;
    }

    public async Task<Unit> Handle(SendTeamInviteCommand request, CancellationToken cancellationToken)
    {
        var isMember = await _repo.IsUserInTeamAsync(request.TeamId, request.InvitingUserId);
        if (!isMember)
            throw new ForbiddenAccessException("You are not a member of this team.");

        await _repo.SendTeamInviteAsync(request.TeamId, request.InvitedUserId, request.InvitingUserId, cancellationToken);

        return Unit.Value;
    }
}