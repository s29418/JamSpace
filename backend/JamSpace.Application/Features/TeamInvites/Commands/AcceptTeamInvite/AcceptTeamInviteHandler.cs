using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Features.TeamInvites.DTOs;
using JamSpace.Application.Features.TeamInvites.Mappers;
using MediatR;

namespace JamSpace.Application.Features.TeamInvites.Commands.AcceptTeamInvite;

public class AcceptTeamInviteHandler : IRequestHandler<AcceptTeamInviteCommand, TeamInviteDto>
{
    private readonly ITeamInviteRepository _teamInviteRepository;

    public AcceptTeamInviteHandler(ITeamInviteRepository teamInviteRepository, ITeamMemberRepository teamMemberRepository)
    {
        _teamInviteRepository = teamInviteRepository;
    }

    public async Task<TeamInviteDto> Handle(AcceptTeamInviteCommand request, CancellationToken cancellationToken)
    {
        var invited = await _teamInviteRepository.GetTeamInviteByIdAsync(request.InviteId, cancellationToken);
        if (invited.InvitedUserId != request.UserId)
            throw new ForbiddenAccessException("Only the invited user can reject the invite.");
        
        var invite = await _teamInviteRepository.AcceptInviteAsync(request.InviteId, request.UserId, cancellationToken);
        return TeamInviteMapper.ToDto(invite);
    }
}
