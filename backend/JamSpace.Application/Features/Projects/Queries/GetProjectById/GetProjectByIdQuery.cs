using JamSpace.Application.Features.Projects.DTOs;
using MediatR;

namespace JamSpace.Application.Features.Projects.Queries.GetProjectById;

public record GetProjectByIdQuery(Guid TeamId, Guid ProjectId, Guid RequestingUserId) : IRequest<ProjectDto>;
