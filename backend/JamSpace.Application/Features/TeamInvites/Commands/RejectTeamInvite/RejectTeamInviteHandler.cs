using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Features.TeamInvites.DTOs;
using JamSpace.Application.Features.TeamInvites.Mappers;
using MediatR;

namespace JamSpace.Application.Features.TeamInvites.Commands.RejectTeamInvite;

public class RejectTeamInviteHandler : IRequestHandler<RejectTeamInviteCommand, TeamInviteDto>
{
    private readonly ITeamInviteRepository _teamInviteRepository;

    public RejectTeamInviteHandler(ITeamInviteRepository repo)
    {
        _teamInviteRepository = repo;
    }

    public async Task<TeamInviteDto> Handle(RejectTeamInviteCommand request, CancellationToken cancellationToken)
    {
        var invited = await _teamInviteRepository.GetTeamInviteByIdAsync(request.InviteId, cancellationToken);
        if (invited.InvitedUserId != request.UserId)
            throw new ForbiddenAccessException("Only the invited user can reject the invite.");
        
        
        var invite = await _teamInviteRepository.RejectInviteAsync(request.InviteId, request.UserId, cancellationToken);
        return TeamInviteMapper.ToDto(invite);
    }
}
