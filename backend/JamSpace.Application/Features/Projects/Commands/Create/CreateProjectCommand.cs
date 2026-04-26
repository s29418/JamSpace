using JamSpace.Application.Features.Projects.DTOs;
using MediatR;

namespace JamSpace.Application.Features.Projects.Commands.Create;

public record CreateProjectCommand(Guid TeamId, string Name) : IRequest<ProjectDto>;