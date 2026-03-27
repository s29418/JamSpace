using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Common.Persistence;
using JamSpace.Application.Features.TeamInvites.DTOs;
using JamSpace.Application.Features.TeamInvites.Mappers;
using JamSpace.Domain.Enums;
using MediatR;

namespace JamSpace.Application.Features.TeamInvites.Commands.CancelTeamInvite;

public class CancelTeamInviteHandler : IRequestHandler<CancelTeamInviteCommand, TeamInviteDto>
{
    private readonly ITeamInviteRepository _teamInviteRepository;
    private readonly ITeamMemberRepository _teamMemberRepository;
    private readonly IUnitOfWork _uow;

    public CancelTeamInviteHandler(
        ITeamInviteRepository teamInviteRepository,
        ITeamMemberRepository teamMemberRepository,
        IUnitOfWork uow)
    {
        _teamInviteRepository = teamInviteRepository;
        _teamMemberRepository = teamMemberRepository;
        _uow = uow;
    }

    public async Task<TeamInviteDto> Handle(CancelTeamInviteCommand request, CancellationToken ct)
    {
        var invite = await _teamInviteRepository.GetByIdWithDetailsAsync(request.TeamInviteId, ct);
        if (invite is null)
            throw new NotFoundException($"Invite with ID '{request.TeamInviteId}' was not found.");

        if (invite.Status != InviteStatus.Pending)
            throw new ConflictException("Invite is no longer pending.");

        var isAdmin = await _teamMemberRepository.HasRequiredRoleAsync(
            invite.TeamId, request.RequestingUserId, FunctionalRole.Admin, ct);

        var isSender = await _teamInviteRepository.WasInviteSentByUserAsync(
            request.TeamInviteId, request.RequestingUserId, ct);

        if (!isAdmin && !isSender)
            throw new ForbiddenAccessException("Only team admin or the user who sent the invite can cancel it.");

        invite.Status = InviteStatus.Cancelled;

        await _uow.SaveChangesAsync(ct);

        return TeamInviteMapper.ToDto(invite);
    }
}