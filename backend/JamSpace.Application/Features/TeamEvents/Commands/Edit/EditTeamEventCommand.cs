using JamSpace.Application.Features.TeamEvents.DTOs;
using MediatR;

namespace JamSpace.Application.Features.TeamEvents.Commands.Edit;

public record EditTeamEventCommand(Guid EventId, Guid TeamId, Guid RequestingUserId, string? Title, string? Description, 
    DateTimeOffset? StartDateTime, int? DurationMinutes) : IRequest<TeamEventDto>;