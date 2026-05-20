using JamSpace.Application.Features.Projects.DTOs;
using MediatR;

namespace JamSpace.Application.Features.Projects.Commands.Edit;

public record EditProjectCommand(
    Guid TeamId,
    Guid ProjectId,
    Guid RequestingUserId,
    string Name,
    string? Description
) : IRequest<ProjectDto>;
