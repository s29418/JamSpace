using JamSpace.Application.Common.Common.Exceptions;
using JamSpace.Application.Common.Features.Teams.Dtos;
using JamSpace.Application.Common.Interfaces;
using MediatR;

namespace JamSpace.Application.Common.Features.Teams.Commands.ChangeTeamMemberFunctionalRole;

public class ChangeTeamMemberFunctionalRoleHandler : IRequestHandler<ChangeTeamMemberFunctionalRoleCommand, TeamMemberDto>
{
    private readonly ITeamRepository _repo;
    
    public ChangeTeamMemberFunctionalRoleHandler(ITeamRepository repo)
    {
        _repo = repo;
    }

    public async Task<TeamMemberDto> Handle(ChangeTeamMemberFunctionalRoleCommand request, CancellationToken ct)
    {
        if (!await _repo.IsUserALeaderAsync(request.TeamId, request.RequestingUserId))
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