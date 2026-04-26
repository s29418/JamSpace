using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Common.Persistence;
using JamSpace.Application.Features.Projects.DTOs;
using JamSpace.Application.Features.Projects.Mappers;
using JamSpace.Domain.Entities;
using MediatR;

namespace JamSpace.Application.Features.Projects.Commands.Create;

public class CreateProjectHandler : IRequestHandler<CreateProjectCommand, ProjectDto>
{
    private readonly IProjectRepository _project;
    private readonly IUnitOfWork _uow;

    public CreateProjectHandler(IProjectRepository project, IUnitOfWork uow)
    {
        _project = project;
        _uow = uow;
    }

    public async Task<ProjectDto> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
    {
        var project = new Project
        {
            Id = Guid.NewGuid(),
            TeamId = request.TeamId,
            Name = request.Name,
            CreatedAt = DateTimeOffset.UtcNow
        };

        await _project.AddAsync(project, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        return ProjectMapper.ToDto(project);
    }
}