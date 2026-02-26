using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Features.Teams.DTOs;
using JamSpace.Application.Features.Teams.Mappers;
using JamSpace.Domain.Enums;
using MediatR;

namespace JamSpace.Application.Features.Teams.Queries.GetTeamById;

public class GetTeamByIdHandler : IRequestHandler<GetTeamByIdQuery, TeamDto>
{
    private readonly ITeamRepository _teamRepository;
    private readonly ITeamMemberRepository _teamMemberRepository;

    public GetTeamByIdHandler(ITeamRepository teamRepository, ITeamMemberRepository teamMemberRepository)
    {
        _teamRepository = teamRepository;
        _teamMemberRepository = teamMemberRepository;
    }

    public async Task<TeamDto> Handle(GetTeamByIdQuery request, CancellationToken ct)
    {
        var isMember = await _teamMemberRepository.HasRequiredRoleAsync(request.TeamId, request.RequestingUserId, FunctionalRole.Member, ct);
        if (!isMember)
            throw new ForbiddenAccessException("You are not a member of this team.");

        var team = await _teamRepository.GetTeamByIdAsync(request.TeamId, ct);

        return TeamMapper.ToDto(team!, request.RequestingUserId);
    }
}