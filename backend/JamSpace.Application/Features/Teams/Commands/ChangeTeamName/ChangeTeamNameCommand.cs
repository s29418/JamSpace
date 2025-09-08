using JamSpace.Application.Features.Teams.DTOs;
using MediatR;

namespace JamSpace.Application.Features.Teams.Commands.ChangeTeamName;

public record ChangeTeamNameCommand(Guid TeamId, Guid RequestingUserId, string Name) : IRequest<TeamDto>;