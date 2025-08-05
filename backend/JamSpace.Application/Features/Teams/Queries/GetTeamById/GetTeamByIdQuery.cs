using JamSpace.Application.Features.Teams.DTOs;
using MediatR;

namespace JamSpace.Application.Features.Teams.Queries.GetTeamById;

public record GetTeamByIdQuery(Guid TeamId, Guid RequestingUserId) : IRequest<TeamDto>;