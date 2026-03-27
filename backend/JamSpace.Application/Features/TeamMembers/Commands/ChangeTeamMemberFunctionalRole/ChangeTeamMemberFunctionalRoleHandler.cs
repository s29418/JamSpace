using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Common.Persistence;
using JamSpace.Application.Features.TeamMembers.DTOs;
using JamSpace.Domain.Enums;
using MediatR;

namespace JamSpace.Application.Features.TeamMembers.Commands.ChangeTeamMemberFunctionalRole;

public sealed class ChangeTeamMemberFunctionalRoleHandler
    : IRequestHandler<ChangeTeamMemberFunctionalRoleCommand, TeamMemberDto>
{
    private readonly ITeamMemberRepository _repo;
    private readonly IUnitOfWork _uow;

    public ChangeTeamMemberFunctionalRoleHandler(ITeamMemberRepository repo, IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public async Task<TeamMemberDto> Handle(ChangeTeamMemberFunctionalRoleCommand request, CancellationToken ct)
    {
        if (!await _repo.HasRequiredRoleAsync(request.TeamId, request.RequestingUserId, FunctionalRole.Leader, ct))
            throw new ForbiddenAccessException("Only team leader can change roles.");

        var member = await _repo.GetByTeamAndUserAsync(request.TeamId, request.UserId, ct)
                     ?? throw new NotFoundException("Team member not found.");
        
        if (member.UserId == request.RequestingUserId && member.Role == FunctionalRole.Leader && request.NewRole != FunctionalRole.Leader)
        {
            var leaders = await _repo.GetLeadersAsync(request.TeamId, ct);
            if (leaders.Count == 1)
                throw new ConflictException("Cannot remove the last team leader.");
        }

        member.Role = request.NewRole;

        await _uow.SaveChangesAsync(ct);

        return new TeamMemberDto
        {
            UserId = member.UserId,
            Username = member.User.UserName,
            Role = member.Role.ToString(),
            MusicalRole = member.MusicalRole,
            UserPictureUrl = member.User.ProfilePictureUrl
        };
    }
}