using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Features.TeamEvents.DTOs;
using JamSpace.Application.Features.TeamEvents.Mappers;
using JamSpace.Domain.Enums;
using MediatR;

namespace JamSpace.Application.Features.TeamEvents.Queries.GetTeamEvents;

public class GetTeamEventsHandler : IRequestHandler<GetTeamEventsQuery, List<TeamEventDto>>
{
    private readonly ITeamEventRepository _events;
    private readonly ITeamMemberRepository _members;
    
    public GetTeamEventsHandler(ITeamEventRepository events, ITeamMemberRepository members)
    {
        _events = events;
        _members = members;
    }

    public async Task<List<TeamEventDto>> Handle(GetTeamEventsQuery request, CancellationToken ct)
    {
        var isParticipant =
            await _members.HasRequiredRoleAsync(request.TeamId, request.RequestingUserId, FunctionalRole.Member, ct);

        if (!isParticipant)
            throw new ForbiddenAccessException("You are not part of this team");

        var events = await _events.GetTeamEvents(request.TeamId, request.From, request.To, ct);

        if (events.Count == 0)
            return new List<TeamEventDto>();

        return events
            .Select(TeamEventMapper.ToDto)
            .ToList();
    }
}