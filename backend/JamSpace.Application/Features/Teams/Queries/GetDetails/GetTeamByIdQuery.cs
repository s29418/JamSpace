using JamSpace.Application.Common.Features.Teams.Dtos;
using MediatR;

namespace JamSpace.Application.Common.Features.Teams.Queries.GetDetails;

public record GetTeamByIdQuery(Guid TeamId, Guid RequestingUserId) : IRequest<TeamDto>;