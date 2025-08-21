using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Features.TeamInvites.DTOs;
using JamSpace.Application.Features.TeamInvites.Mappers;
using MediatR;

namespace JamSpace.Application.Features.TeamInvites.Commands.AcceptTeamInvite;

public class AcceptTeamInviteHandler : IRequestHandler<AcceptTeamInviteCommand, TeamInviteDto>
{
    private readonly ITeamInviteRepository _teamInviteRepository;
    private readonly ITeamMemberRepository _teamMemberRepository;

    public AcceptTeamInviteHandler(ITeamInviteRepository teamInviteRepository, ITeamMemberRepository teamMemberRepository)
    {
        _teamInviteRepository = teamInviteRepository;
        _teamMemberRepository = teamMemberRepository;
    }

    public async Task<TeamInviteDto> Handle(AcceptTeamInviteCommand request, CancellationToken ct)
    {
        var invited = await _teamInviteRepository.GetTeamInviteByIdAsync(request.InviteId, ct);
        if (invited.InvitedUserId != request.UserId)
            throw new ForbiddenAccessException("Only the invited user can reject the invite.");
        
        if (await _teamMemberRepository.IsUserInTeamAsync(invited.TeamId, request.UserId, ct))
            throw new ConflictException("User is already in the team.");
        
        var invite = await _teamInviteRepository.AcceptInviteAsync(request.InviteId, request.UserId, ct);
        return TeamInviteMapper.ToDto(invite);
    }
}
