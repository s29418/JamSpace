using MediatR;

namespace JamSpace.Application.Features.Projects.Commands.Delete;

public record DeleteProjectCommand(Guid TeamId, Guid ProjectId, Guid RequestingUserId) : IRequest;
