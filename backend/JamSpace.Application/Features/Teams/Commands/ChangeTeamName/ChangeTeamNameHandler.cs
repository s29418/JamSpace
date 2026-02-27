using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Common.Persistence;
using JamSpace.Application.Features.Teams.DTOs;
using JamSpace.Application.Features.Teams.Mappers;
using JamSpace.Domain.Enums;
using MediatR;

namespace JamSpace.Application.Features.Teams.Commands.ChangeTeamName;

public class ChangeTeamNameHandler : IRequestHandler<ChangeTeamNameCommand, TeamDto>
{
    private readonly ITeamRepository _teams;
    private readonly ITeamMemberRepository _teamMembers;
    private readonly IUnitOfWork _uow;

    public ChangeTeamNameHandler(ITeamRepository teams, ITeamMemberRepository teamMembers, IUnitOfWork uow)
    {
        _teams = teams;
        _teamMembers = teamMembers;
        _uow = uow;
    }

    public async Task<TeamDto> Handle(ChangeTeamNameCommand request, CancellationToken ct)
    {
        if (!await _teamMembers.HasRequiredRoleAsync(request.TeamId, request.RequestingUserId, FunctionalRole.Admin, ct))
            throw new ForbiddenAccessException("Only team leaders and admins can change team names.");

        var team = await _teams.GetByIdAsync(request.TeamId, ct)
                   ?? throw new NotFoundException("Team not found.");

        team.Name = request.Name;

        await _uow.SaveChangesAsync(ct);

        return TeamMapper.ToDto(team, request.RequestingUserId);
    }
}