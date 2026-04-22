using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Features.TeamEvents.DTOs;
using JamSpace.Application.Features.TeamEvents.Mappers;
using JamSpace.Domain.Enums;
using MediatR;

namespace JamSpace.Application.Features.TeamEvents.Queries.GetTeamEvents;

public class GetTeamEventsHandler : IRequestHandler<GetTeamEventsQuery, IReadOnlyList<TeamEventDto>>
{
    private readonly ITeamEventRepository _events;
    private readonly ITeamMemberRepository _members;
    
    public GetTeamEventsHandler(ITeamEventRepository events, ITeamMemberRepository members)
    {
        _events = events;
        _members = members;
    }

    public async Task<IReadOnlyList<TeamEventDto>> Handle(GetTeamEventsQuery request, CancellationToken ct)
    {
        var isParticipant =
            await _members.HasRequiredRoleAsync(request.TeamId, request.RequestingUserId, FunctionalRole.Member, ct);

        if (!isParticipant)
            throw new ForbiddenAccessException("You are not part of this team");

        var events = await _events.GetTeamEventsAsync(request.TeamId, request.From, request.To, ct);
        
        return events
            .Select(TeamEventMapper.ToDto)
            .ToList();
    }
}