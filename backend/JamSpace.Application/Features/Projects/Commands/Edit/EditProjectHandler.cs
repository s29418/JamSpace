using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Common.Persistence;
using JamSpace.Application.Features.Projects.DTOs;
using JamSpace.Application.Features.Projects.Mappers;
using JamSpace.Domain.Enums;
using MediatR;

namespace JamSpace.Application.Features.Projects.Commands.Edit;

public class EditProjectHandler : IRequestHandler<EditProjectCommand, ProjectDto>
{
    private readonly IProjectRepository _projects;
    private readonly ITeamMemberRepository _members;
    private readonly IUnitOfWork _uow;

    public EditProjectHandler(IProjectRepository projects, ITeamMemberRepository members, IUnitOfWork uow)
    {
        _projects = projects;
        _members = members;
        _uow = uow;
    }

    public async Task<ProjectDto> Handle(EditProjectCommand request, CancellationToken ct)
    {
        var project = await _projects.GetByIdAsync(request.ProjectId, ct);
        if (project is null || project.TeamId != request.TeamId)
            throw new NotFoundException("Project not found.");

        var canEdit =
            await _members.HasRequiredRoleAsync(request.TeamId, request.RequestingUserId, FunctionalRole.Admin, ct);

        if (!canEdit)
            throw new ForbiddenAccessException("Only team leaders and admins can edit projects.");

        project.Name = request.Name.Trim();
        project.Description = Normalize(request.Description);
        project.UpdatedAt = DateTimeOffset.UtcNow;

        await _uow.SaveChangesAsync(ct);

        return ProjectMapper.ToDto(project);
    }

    private static string? Normalize(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
