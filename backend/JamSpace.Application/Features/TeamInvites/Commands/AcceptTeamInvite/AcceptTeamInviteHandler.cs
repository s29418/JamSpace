using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Common.Persistence;
using JamSpace.Application.Features.TeamInvites.DTOs;
using JamSpace.Application.Features.TeamInvites.Mappers;
using JamSpace.Domain.Entities;
using JamSpace.Domain.Enums;
using MediatR;

namespace JamSpace.Application.Features.TeamInvites.Commands.AcceptTeamInvite;

public class AcceptTeamInviteHandler : IRequestHandler<AcceptTeamInviteCommand, TeamInviteDto>
{
    private readonly ITeamInviteRepository _teamInviteRepository;
    private readonly ITeamMemberRepository _teamMemberRepository;
    private readonly IUnitOfWork _uow;

    public AcceptTeamInviteHandler(
        ITeamInviteRepository teamInviteRepository,
        ITeamMemberRepository teamMemberRepository,
        IUnitOfWork uow)
    {
        _teamInviteRepository = teamInviteRepository;
        _teamMemberRepository = teamMemberRepository;
        _uow = uow;
    }

    public async Task<TeamInviteDto> Handle(AcceptTeamInviteCommand request, CancellationToken ct)
    {
        var invite = await _teamInviteRepository.GetByIdWithDetailsAsync(request.InviteId, ct);
        if (invite is null)
            throw new NotFoundException($"Invite with ID '{request.InviteId}' was not found.");

        if (invite.InvitedUserId != request.UserId)
            throw new ForbiddenAccessException("Only the invited user can accept the invite.");

        if (invite.Status != InviteStatus.Pending)
            throw new ConflictException("Invite is no longer pending.");

        var alreadyInTeam = await _teamMemberRepository.HasRequiredRoleAsync(
            invite.TeamId, request.UserId, FunctionalRole.Member, ct);

        if (alreadyInTeam)
            throw new ConflictException("User is already in the team.");

        invite.Status = InviteStatus.Accepted;
        
        await _teamMemberRepository.AddAsync(new TeamMember
        {
            TeamId = invite.TeamId,
            UserId = request.UserId
        }, ct);

        await _uow.SaveChangesAsync(ct);

        return TeamInviteMapper.ToDto(invite);
    }
}