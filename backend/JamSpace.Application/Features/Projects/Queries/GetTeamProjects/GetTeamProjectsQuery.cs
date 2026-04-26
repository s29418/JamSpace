using JamSpace.Application.Features.Projects.DTOs;
using MediatR;

namespace JamSpace.Application.Features.Projects.Queries.GetTeamProjects;

public record GetTeamProjectsQuery(Guid RequestingUserId, Guid TeamId) : IRequest<IReadOnlyList<ProjectDto>>;