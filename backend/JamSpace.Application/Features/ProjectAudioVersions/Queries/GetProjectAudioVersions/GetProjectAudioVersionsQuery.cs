using JamSpace.Application.Features.ProjectAudioVersions.DTOs;
using MediatR;

namespace JamSpace.Application.Features.ProjectAudioVersions.Queries.GetProjectAudioVersions;

public record GetProjectAudioVersionsQuery(Guid TeamId, Guid ProjectId, Guid RequestingUserId) 
    : IRequest<IReadOnlyList<ProjectAudioVersionDto>>;
