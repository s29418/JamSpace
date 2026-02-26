using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Features.TeamMembers.DTOs;
using JamSpace.Domain.Enums;
using MediatR;

namespace JamSpace.Application.Features.TeamMembers.Commands.EditTeamMemberMusicalRole;

public class EditTeamMemberMusicalRoleHandler : IRequestHandler<EditTeamMemberMusicalRoleCommand, TeamMemberDto>
{
    private readonly ITeamMemberRepository _repo;

    public EditTeamMemberMusicalRoleHandler(ITeamMemberRepository repo)
    {
        _repo = repo;
    }

    public async Task<TeamMemberDto> Handle(EditTeamMemberMusicalRoleCommand request, CancellationToken ct)
    {
        if (!await _repo.HasRequiredRoleAsync(request.TeamId, request.RequestingUserId, FunctionalRole.Admin, ct))
            throw new ForbiddenAccessException("Only team leaders and admins can edit members musial roles.");
        
        var teamMember = await _repo.EditTeamMemberMusicalRole(request.TeamId, request.UserId, request.MusicalRole, ct);

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