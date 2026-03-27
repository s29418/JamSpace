using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Common.Persistence;
using JamSpace.Application.Features.TeamInvites.DTOs;
using JamSpace.Application.Features.TeamInvites.Mappers;
using JamSpace.Domain.Entities;
using JamSpace.Domain.Enums;
using MediatR;

namespace JamSpace.Application.Features.TeamInvites.Commands.SendTeamInvite;

public class SendTeamInviteHandler : IRequestHandler<SendTeamInviteCommand, TeamInviteDto>
{
    private readonly ITeamInviteRepository _teamInviteRepository;
    private readonly ITeamMemberRepository _teamMemberRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _uow;

    public SendTeamInviteHandler(
        ITeamInviteRepository teamInviteRepository,
        ITeamMemberRepository teamMemberRepository,
        IUserRepository userRepository,
        IUnitOfWork uow)
    {
        _teamInviteRepository = teamInviteRepository;
        _teamMemberRepository = teamMemberRepository;
        _userRepository = userRepository;
        _uow = uow;
    }

    public async Task<TeamInviteDto> Handle(SendTeamInviteCommand request, CancellationToken ct)
    {
        var isMember = await _teamMemberRepository.HasRequiredRoleAsync(
            request.TeamId, request.InvitingUserId, FunctionalRole.Member, ct);

        if (!isMember)
            throw new ForbiddenAccessException("You are not a member of this team.");

        var invitedUserId = await _userRepository.GetUserIdByUsernameAsync(request.InvitedUserName, ct);
        if (invitedUserId is null)
            throw new NotFoundException($"User '{request.InvitedUserName}' not found.");

        var alreadyInTeam = await _teamMemberRepository.HasRequiredRoleAsync(
            request.TeamId, invitedUserId.Value, FunctionalRole.Member, ct);

        if (alreadyInTeam)
            throw new ConflictException("User is already in the team.");

        var pendingExists = await _teamInviteRepository.ExistsPendingInviteAsync(
            request.TeamId, invitedUserId.Value, ct);

        if (pendingExists)
            throw new ConflictException("Invite already exists.");

        var invite = new TeamInvite
        {
            Id = Guid.NewGuid(),
            TeamId = request.TeamId,
            InvitedUserId = invitedUserId.Value,
            InvitedByUserId = request.InvitingUserId,
            CreatedAt = DateTime.UtcNow,
            Status = InviteStatus.Pending
        };

        await _teamInviteRepository.AddAsync(invite, ct);
        await _uow.SaveChangesAsync(ct);
        
        var full = await _teamInviteRepository.GetByIdWithDetailsAsync(invite.Id, ct)
                   ?? throw new NotFoundException($"Invite with ID '{invite.Id}' was not found.");

        return TeamInviteMapper.ToDto(full);
    }
}