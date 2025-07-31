using JamSpace.Application.Common.Common.Exceptions;
using JamSpace.Application.Common.Features.Teams.Dtos;
using JamSpace.Application.Common.Interfaces;
using MediatR;

namespace JamSpace.Application.Common.Features.Teams.Commands.EditTeamMemberMusicalRole;

public class EditTeamMemberMusicalRoleHandler : IRequestHandler<EditTeamMemberMusicalRoleCommand, TeamMemberDto>
{
    private readonly ITeamRepository _repo;

    public EditTeamMemberMusicalRoleHandler(ITeamRepository repo)
    {
        _repo = repo;
    }

    public async Task<TeamMemberDto> Handle(EditTeamMemberMusicalRoleCommand request, CancellationToken cancellationToken)
    {
        if (!await _repo.IsUserALeaderAsync(request.TeamId, request.RequestingUserId) &&
            !await _repo.IsUserAnAdminAsync(request.TeamId, request.RequestingUserId))
            throw new ForbiddenAccessException("Only team leaders and admins can edit members musial roles.");
        
        var teamMember = await _repo.EditTeamMemberMusicalRole(request.TeamId, request.UserId, request.MusicalRole, cancellationToken);

        return new TeamMemberDto
        {
            UserId = teamMember.UserId,
            Username = teamMember.User.UserName,
            Role = teamMember.Role.ToString(),
            MusicalRole = teamMember.MusicalRole,
            UserPictureUrl = teamMember.User.ProfilePictureUrl
        };
    }
}