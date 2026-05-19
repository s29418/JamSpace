using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Common.Persistence;
using JamSpace.Domain.Enums;
using MediatR;

namespace JamSpace.Application.Features.ProjectAudioVersions.Commands.Delete;

public class DeleteProjectAudioVersionHandler : IRequestHandler<DeleteProjectAudioVersionCommand>
{
    private readonly IProjectRepository _projects;
    private readonly IProjectAudioVersionRepository _versions;
    private readonly IProjectNoteRepository _notes;
    private readonly ITeamMemberRepository _members;
    private readonly IFileStorageService _files;
    private readonly IUnitOfWork _uow;

    public DeleteProjectAudioVersionHandler(
        IProjectRepository projects,
        IProjectAudioVersionRepository versions,
        IProjectNoteRepository notes,
        ITeamMemberRepository members,
        IFileStorageService files,
        IUnitOfWork uow)
    {
        _projects = projects;
        _versions = versions;
        _notes = notes;
        _members = members;
        _files = files;
        _uow = uow;
    }

    public async Task Handle(DeleteProjectAudioVersionCommand request, CancellationToken ct)
    {
        var project = await _projects.GetByIdAsync(request.ProjectId, ct);
        if (project is null || project.TeamId != request.TeamId)
            throw new NotFoundException("Project not found.");

        var version = await _versions.GetByIdAsync(request.VersionId, ct);
        if (version is null || version.ProjectId != request.ProjectId)
            throw new NotFoundException("Audio version not found.");

        var isTeamMember =
            await _members.HasRequiredRoleAsync(request.TeamId, request.RequestingUserId, FunctionalRole.Member, ct);

        if (!isTeamMember)
            throw new ForbiddenAccessException("You are not part of this team.");

        var canDelete = version.CreatedById == request.RequestingUserId ||
                        await _members.HasRequiredRoleAsync(
                            request.TeamId,
                            request.RequestingUserId,
                            FunctionalRole.Admin,
                            ct);

        if (!canDelete)
            throw new ForbiddenAccessException("You must be an admin to delete audio versions uploaded by other users.");

        var fileUrl = version.FileUrl;

        var notes = await _notes.GetByAudioVersionIdAsync(version.Id, ct);
        var updatedAt = DateTimeOffset.UtcNow;

        foreach (var note in notes)
        {
            note.AudioVersionId = null;
            note.AudioVersionNameSnapshot = version.Name;
            note.IsAudioVersionDeleted = true;
            note.UpdatedAt = updatedAt;
        }

        _versions.Remove(version);
        await _uow.SaveChangesAsync(ct);

        try
        {
            await _files.DeleteAsync(fileUrl, ct);
        }
        catch
        {
            // ignored
        }
    }
}
