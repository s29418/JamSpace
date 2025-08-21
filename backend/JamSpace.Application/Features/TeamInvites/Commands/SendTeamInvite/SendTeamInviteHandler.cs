using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Features.TeamInvites.DTOs;
using JamSpace.Application.Features.TeamInvites.Mappers;
using MediatR;

namespace JamSpace.Application.Features.TeamInvites.Commands.SendTeamInvite;

public class SendTeamInviteHandler : IRequestHandler<SendTeamInviteCommand, TeamInviteDto>
{
    private readonly ITeamInviteRepository _teamInviteRepository;
    private readonly ITeamMemberRepository _teamMemberRepository;
    private readonly IUserRepository _userRepository;

    public SendTeamInviteHandler(ITeamInviteRepository teamInviteRepository, ITeamMemberRepository teamMemberRepository, IUserRepository userRepository)
    {
        _teamInviteRepository = teamInviteRepository;
        _teamMemberRepository = teamMemberRepository;
        _userRepository = userRepository;
    }

    public async Task<TeamInviteDto> Handle(SendTeamInviteCommand request, CancellationToken ct)
    {
        var isMember = await _teamMemberRepository.IsUserInTeamAsync(request.TeamId, request.InvitingUserId, ct);
        if (!isMember)
            throw new ForbiddenAccessException("You are not a member of this team.");

        var invitedUserId = await _userRepository.GetUserIdByUsernameAsync(request.InvitedUserName, ct);
        if (invitedUserId is null)
            throw new NotFoundException($"User '{request.InvitedUserName}' not found.");

        var invite = 
            await _teamInviteRepository.SendTeamInviteAsync(request.TeamId, invitedUserId.Value, 
                request.InvitingUserId, ct);
        
        return TeamInviteMapper.ToDto(invite);
    }
}