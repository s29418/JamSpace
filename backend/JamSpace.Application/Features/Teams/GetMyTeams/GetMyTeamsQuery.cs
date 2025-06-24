using JamSpace.Application.Features.Teams.Dtos;
using MediatR;

namespace JamSpace.Application.Features.Teams.GetMyTeams;

public record GetMyTeamsQuery(Guid UserId) : IRequest<List<TeamDto>>;