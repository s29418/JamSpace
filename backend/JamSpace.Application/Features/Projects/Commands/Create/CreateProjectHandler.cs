using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Common.Persistence;
using JamSpace.Application.Features.Projects.DTOs;
using JamSpace.Application.Features.Projects.Mappers;
using JamSpace.Domain.Entities;
using JamSpace.Domain.Enums;
using MediatR;

namespace JamSpace.Application.Features.Projects.Commands.Create;

public class CreateProjectHandler : IRequestHandler<CreateProjectCommand, ProjectDto>
{
    private readonly IProjectRepository _project;
    private readonly ITeamMemberRepository _member;
    private readonly IUnitOfWork _uow;

    public CreateProjectHandler(IProjectRepository project, IUnitOfWork uow, ITeamMemberRepository member)
    {
        _project = project;
        _uow = uow;
        _member = member;
    }

    public async Task<ProjectDto> Handle(CreateProjectCommand request, CancellationToken ct)
    {
        var isTeamMember =
            await _member.HasRequiredRoleAsync(request.TeamId, request.RequestingUserId, FunctionalRole.Member, ct);

        if (!isTeamMember)
            throw new ForbiddenAccessException("You are not a member of this team");
        
        var project = new Project
        {
            Id = Guid.NewGuid(),
            TeamId = request.TeamId,
            Name = request.Name,
            CreatedAt = DateTimeOffset.UtcNow
        };

        await _project.AddAsync(project, ct);
        await _uow.SaveChangesAsync(ct);

        return ProjectMapper.ToDto(project);
    }
}