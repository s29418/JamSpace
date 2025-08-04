using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Features.Teams.Dtos;
using JamSpace.Application.Features.Teams.Mappers;
using MediatR;

namespace JamSpace.Application.Features.Teams.Commands.SendTeamInvite;

public class SendTeamInviteHandler : IRequestHandler<SendTeamInviteCommand, TeamInviteDto>
{
    private readonly ITeamRepository _repo;
    private readonly IUserRepository _userRepo;

    public SendTeamInviteHandler(ITeamRepository repo, IUserRepository userRepo)
    {
        _repo = repo;
        _userRepo = userRepo;
    }

    public async Task<TeamInviteDto> Handle(SendTeamInviteCommand request, CancellationToken cancellationToken)
    {
        var isMember = await _repo.IsUserInTeamAsync(request.TeamId, request.InvitingUserId);
        if (!isMember)
            throw new ForbiddenAccessException("You are not a member of this team.");

        var invitedUserId = await _userRepo.GetUserIdByUsernameAsync(request.InvitedUserName, cancellationToken);
        if (invitedUserId is null)
            throw new NotFoundException($"User '{request.InvitedUserName}' not found.");

        var invite = 
            await _repo.SendTeamInviteAsync(request.TeamId, invitedUserId.Value, 
                request.InvitingUserId, cancellationToken);
        
        return TeamInviteMapper.ToDto(invite);
    }
}