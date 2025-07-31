using JamSpace.Application.Common.Features.Teams.Dtos;
using MediatR;

namespace JamSpace.Application.Common.Features.Teams.Queries.GetMyTeams;

public record GetMyTeamsQuery(Guid UserId) : IRequest<List<TeamDto>>;