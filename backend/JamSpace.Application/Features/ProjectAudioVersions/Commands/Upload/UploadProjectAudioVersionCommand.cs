using JamSpace.Application.Common.Models;
using JamSpace.Application.Features.ProjectAudioVersions.DTOs;
using MediatR;

namespace JamSpace.Application.Features.ProjectAudioVersions.Commands.Upload;

public record UploadProjectAudioVersionCommand(
    Guid TeamId,
    Guid ProjectId,
    Guid RequestingUserId,
    string Name,
    int? DurationSeconds,
    FileUpload File
) : IRequest<ProjectAudioVersionDto>;
