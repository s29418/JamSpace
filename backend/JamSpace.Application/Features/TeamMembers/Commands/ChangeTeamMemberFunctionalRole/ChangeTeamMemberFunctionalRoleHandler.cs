using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Features.TeamMembers.DTOs;
using JamSpace.Domain.Enums;
using MediatR;

namespace JamSpace.Application.Features.TeamMembers.Commands.ChangeTeamMemberFunctionalRole;

public class ChangeTeamMemberFunctionalRoleHandler : IRequestHandler<ChangeTeamMemberFunctionalRoleCommand, TeamMemberDto>
{
    private readonly ITeamMemberRepository _repo;
    
    public ChangeTeamMemberFunctionalRoleHandler(ITeamMemberRepository repo)
    {
        _repo = repo;
    }

    public async Task<TeamMemberDto> Handle(ChangeTeamMemberFunctionalRoleCommand request, CancellationToken ct)
    {
        if (!await _repo.HasRequiredRoleAsync(request.TeamId, request.RequestingUserId, FunctionalRole.Leader, ct))
            throw new ForbiddenAccessException("Only team leader can change roles.");

        if (request.RequestingUserId == request.UserId)
        {
            var leaders = await _repo.GetLeadersAsync(request.TeamId, ct);
            if (leaders.Count == 1)
                throw new ConflictException("Cannot remove the last team leader.");
        }

        var updated = await _repo.ChangeTeamMemberFunctionalRoleAsync(
            request.TeamId, request.UserId, request.NewRole, ct);

        return new TeamMemberDto
        {
            UserId = updated.UserId,
            Username = updated.User.UserName,
            Role = updated.Role.ToString(),
            MusicalRole = updated.MusicalRole,
            UserPictureUrl = updated.User.ProfilePictureUrl
        };
    }
}