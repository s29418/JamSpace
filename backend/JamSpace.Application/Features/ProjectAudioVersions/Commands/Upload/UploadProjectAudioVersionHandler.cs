using JamSpace.Application.Common.Enums;
using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Common.Persistence;
using JamSpace.Application.Features.ProjectAudioVersions.DTOs;
using JamSpace.Application.Features.ProjectAudioVersions.Mappers;
using JamSpace.Domain.Entities;
using JamSpace.Domain.Enums;
using MediatR;

namespace JamSpace.Application.Features.ProjectAudioVersions.Commands.Upload;

public class UploadProjectAudioVersionHandler
    : IRequestHandler<UploadProjectAudioVersionCommand, ProjectAudioVersionDto>
{
    private readonly IProjectRepository _projects;
    private readonly IProjectAudioVersionRepository _versions;
    private readonly ITeamMemberRepository _members;
    private readonly IFileStorageService _files;
    private readonly IUnitOfWork _uow;

    public UploadProjectAudioVersionHandler(
        IProjectRepository projects,
        IProjectAudioVersionRepository versions,
        ITeamMemberRepository members,
        IFileStorageService files,
        IUnitOfWork uow)
    {
        _projects = projects;
        _versions = versions;
        _members = members;
        _files = files;
        _uow = uow;
    }

    public async Task<ProjectAudioVersionDto> Handle(
        UploadProjectAudioVersionCommand request,
        CancellationToken ct)
    {
        var project = await _projects.GetByIdAsync(request.ProjectId, ct);
        if (project is null || project.TeamId != request.TeamId)
            throw new NotFoundException("Project not found.");

        var isTeamMember =
            await _members.HasRequiredRoleAsync(request.TeamId, request.RequestingUserId, FunctionalRole.Member, ct);

        if (!isTeamMember)
            throw new ForbiddenAccessException("You are not part of this team.");

        var versionId = Guid.NewGuid();
        string? uploadedUrl = null;

        try
        {
            uploadedUrl = await _files.UploadAsync(
                request.File,
                StorageObjectType.ProjectAudioVersionAudio,
                versionId,
                ct);

            var version = new ProjectAudioVersion
            {
                Id = versionId,
                ProjectId = request.ProjectId,
                CreatedById = request.RequestingUserId,
                Name = request.Name.Trim(),
                FileUrl = uploadedUrl,
                OriginalFileName = request.File.FileName,
                ContentType = request.File.ContentType ?? "application/octet-stream",
                Length = request.File.Length,
                DurationSeconds = request.DurationSeconds,
                CreatedAt = DateTimeOffset.UtcNow
            };

            await _versions.AddAsync(version, ct);
            await _uow.SaveChangesAsync(ct);

            var savedVersion = await _versions.GetByIdAsync(versionId, ct);
            return ProjectAudioVersionMapper.ToDto(savedVersion!);
        }
        catch
        {
            if (!string.IsNullOrWhiteSpace(uploadedUrl))
            {
                try
                {
                    await _files.DeleteAsync(uploadedUrl, ct);
                }
                catch
                {
                    // ignored
                }
            }

            throw;
        }
    }
}
