using JamSpace.Application.Features.TeamEvents.DTOs;
using MediatR;

namespace JamSpace.Application.Features.TeamEvents.Queries.GetTeamEvents;

public record GetTeamEventsQuery(Guid TeamId, Guid RequestingUserId, DateTimeOffset From, DateTimeOffset To) 
    : IRequest<IReadOnlyList<TeamEventDto>>;
