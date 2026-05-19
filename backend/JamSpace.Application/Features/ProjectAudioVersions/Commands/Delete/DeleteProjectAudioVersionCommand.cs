using MediatR;

namespace JamSpace.Application.Features.ProjectAudioVersions.Commands.Delete;

public record DeleteProjectAudioVersionCommand(
    Guid TeamId,
    Guid ProjectId,
    Guid VersionId,
    Guid RequestingUserId
) : IRequest;
