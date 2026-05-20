using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Common.Persistence;
using JamSpace.Domain.Enums;
using MediatR;

namespace JamSpace.Application.Features.Projects.Commands.Delete;

public class DeleteProjectHandler : IRequestHandler<DeleteProjectCommand>
{
    private readonly IProjectRepository _projects;
    private readonly ITeamMemberRepository _members;
    private readonly IFileStorageService _files;
    private readonly IUnitOfWork _uow;

    public DeleteProjectHandler(
        IProjectRepository projects,
        ITeamMemberRepository members,
        IFileStorageService files,
        IUnitOfWork uow)
    {
        _projects = projects;
        _members = members;
        _files = files;
        _uow = uow;
    }

    public async Task Handle(DeleteProjectCommand request, CancellationToken ct)
    {
        var project = await _projects.GetByIdWithAudioVersionsAsync(request.ProjectId, ct);
        if (project is null || project.TeamId != request.TeamId)
            throw new NotFoundException("Project not found.");

        var canDelete =
            await _members.HasRequiredRoleAsync(request.TeamId, request.RequestingUserId, FunctionalRole.Admin, ct);

        if (!canDelete)
            throw new ForbiddenAccessException("Only team leaders and admins can delete projects.");

        var fileUrls = project.AudioVersions
            .Select(v => v.FileUrl)
            .Where(url => !string.IsNullOrWhiteSpace(url))
            .ToList();

        if (!string.IsNullOrWhiteSpace(project.PictureUrl))
            fileUrls.Add(project.PictureUrl);

        _projects.Remove(project);
        await _uow.SaveChangesAsync(ct);

        foreach (var fileUrl in fileUrls)
        {
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
}
