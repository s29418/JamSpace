using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Common.Persistence;
using JamSpace.Application.Features.TeamMembers.DTOs;
using JamSpace.Domain.Enums;
using MediatR;

namespace JamSpace.Application.Features.TeamMembers.Commands.EditTeamMemberMusicalRole;

public sealed class EditTeamMemberMusicalRoleHandler
    : IRequestHandler<EditTeamMemberMusicalRoleCommand, TeamMemberDto>
{
    private readonly ITeamMemberRepository _repo;
    private readonly IUnitOfWork _uow;

    public EditTeamMemberMusicalRoleHandler(ITeamMemberRepository repo, IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public async Task<TeamMemberDto> Handle(EditTeamMemberMusicalRoleCommand request, CancellationToken ct)
    {
        if (!await _repo.HasRequiredRoleAsync(request.TeamId, request.RequestingUserId, FunctionalRole.Admin, ct))
            throw new ForbiddenAccessException("Only team leaders and admins can edit members musical roles.");

        var member = await _repo.GetByTeamAndUserAsync(request.TeamId, request.UserId, ct)
                     ?? throw new NotFoundException("Team member not found.");

        member.MusicalRole = request.MusicalRole;

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