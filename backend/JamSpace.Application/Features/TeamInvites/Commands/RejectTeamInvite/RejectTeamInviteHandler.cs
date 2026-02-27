using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Common.Persistence;
using JamSpace.Application.Features.TeamInvites.DTOs;
using JamSpace.Application.Features.TeamInvites.Mappers;
using JamSpace.Domain.Enums;
using MediatR;

namespace JamSpace.Application.Features.TeamInvites.Commands.RejectTeamInvite;

public class RejectTeamInviteHandler : IRequestHandler<RejectTeamInviteCommand, TeamInviteDto>
{
    private readonly ITeamInviteRepository _teamInviteRepository;
    private readonly IUnitOfWork _uow;

    public RejectTeamInviteHandler(ITeamInviteRepository repo, IUnitOfWork uow)
    {
        _teamInviteRepository = repo;
        _uow = uow;
    }

    public async Task<TeamInviteDto> Handle(RejectTeamInviteCommand request, CancellationToken ct)
    {
        var invite = await _teamInviteRepository.GetByIdWithDetailsAsync(request.InviteId, ct);
        if (invite is null)
            throw new NotFoundException($"Invite with ID '{request.InviteId}' was not found.");

        if (invite.InvitedUserId != request.UserId)
            throw new ForbiddenAccessException("Only the invited user can reject the invite.");

        if (invite.Status != InviteStatus.Pending)
            throw new ConflictException("Invite is no longer pending.");

        invite.Status = InviteStatus.Rejected;

        await _uow.SaveChangesAsync(ct);

        return TeamInviteMapper.ToDto(invite);
    }
}