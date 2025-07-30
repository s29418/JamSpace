using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Interfaces;
using MediatR;

namespace JamSpace.Application.Features.Teams.Commands.SendTeamInvite;

public class SendTeamInviteHandler : IRequestHandler<SendTeamInviteCommand, Unit>
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

        var invitedUserId = await _repo.GetUserIdByUsernameAsync(request.InvitedUserName, cancellationToken);
        if (invitedUserId is null)
            throw new NotFoundException($"User '{request.InvitedUserName}' not found.");

        await _repo.SendTeamInviteAsync(request.TeamId, invitedUserId.Value, request.InvitingUserId, cancellationToken);

        return Unit.Value;
    }
}