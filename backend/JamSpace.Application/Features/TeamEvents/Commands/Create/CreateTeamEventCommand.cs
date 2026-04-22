using JamSpace.Application.Features.TeamEvents.DTOs;
using MediatR;

namespace JamSpace.Application.Features.TeamEvents.Commands.Create;

public record CreateTeamEventCommand(Guid RequestingUserId, Guid TeamId, string Title, string? Description, 
    DateTimeOffset StartDateTime, int DurationMinutes) : IRequest<TeamEventDto>;