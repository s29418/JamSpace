using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Features.TeamInvites.DTOs;
using JamSpace.Application.Features.TeamInvites.Mappers;
using MediatR;

namespace JamSpace.Application.Features.TeamInvites.Commands.CancelTeamInvite;

public class CancelTeamInviteHandler : IRequestHandler<CancelTeamInviteCommand, TeamInviteDto>
{
    private readonly ITeamInviteRepository _teamInviteRepository;
    private readonly ITeamMemberRepository _teamMemberRepository;
    
    public CancelTeamInviteHandler(ITeamInviteRepository teamInviteRepository, ITeamMemberRepository teamMemberRepository)
    {
        _teamInviteRepository = teamInviteRepository;
        _teamMemberRepository = teamMemberRepository;
    }

    public async Task<TeamInviteDto> Handle(CancelTeamInviteCommand request, CancellationToken ct)
    {
        var teamInvite = await _teamInviteRepository.GetTeamInviteByIdAsync(request.TeamInviteId, ct);
        var teamId = teamInvite.TeamId;
        
        var hasPermission =
            await _teamMemberRepository.IsUserALeaderAsync(teamId, request.RequestingUserId, ct) ||
            await _teamMemberRepository.IsUserAnAdminAsync(teamId, request.RequestingUserId, ct) ||
            await _teamInviteRepository.WasInviteSentByUserAsync(request.TeamInviteId, request.RequestingUserId, ct);

        if (!hasPermission)
            throw new ForbiddenAccessException("Only team leader, admin or user who sent the invite can cancel it.");

        var invite = await _teamInviteRepository.CancelTeamInviteAsync(request.TeamInviteId, request.RequestingUserId, ct);
        return TeamInviteMapper.ToDto(invite);
    }
}