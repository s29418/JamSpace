using JamSpace.Application.Features.Teams.Dtos;
using MediatR;

namespace JamSpace.Application.Features.Teams.GetDetails;

public record GetTeamByIdQuery(Guid TeamId, Guid RequestingUserId) : IRequest<TeamDto>;