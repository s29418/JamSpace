using JamSpace.Application.Common.Enums;
using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Common.Persistence;
using JamSpace.Domain.Enums;
using MediatR;

namespace JamSpace.Application.Features.Projects.Commands.UploadProjectPicture;

public class UploadProjectPictureHandler : IRequestHandler<UploadProjectPictureCommand, string>
{
    private readonly IProjectRepository _projects;
    private readonly ITeamRepository _teams;
    private readonly ITeamMemberRepository _members;
    private readonly IUnitOfWork _uow;
    private readonly IFileStorageService _files;


    public UploadProjectPictureHandler(IProjectRepository projects, ITeamRepository teams, ITeamMemberRepository members, IUnitOfWork uow, IFileStorageService files)
    {
        _projects = projects;
        _teams = teams;
        _members = members;
        _uow = uow;
        _files = files;
    }

    public async Task<string> Handle(UploadProjectPictureCommand c, CancellationToken ct)
    {
        _ = await _teams.GetByIdAsync(c.TeamId, ct)
            ?? throw new NotFoundException("Team not found.");

        var project = await _projects.GetByIdAsync(c.ProjectId, ct)
                      ?? throw new NotFoundException("Project not found.");

        var isParticipant =
            await _members.HasRequiredRoleAsync(c.TeamId, c.RequestingUserId, FunctionalRole.Member, ct);
        
        if (!isParticipant)
            throw new ForbiddenAccessException("You are not part of this team.");

        var oldPictureUrl = project.PictureUrl;
        string? newPictureUrl = null;

        try
        {
            newPictureUrl = await _files.UploadAsync(c.File, StorageObjectType.ProjectPicture, c.ProjectId, ct);

            project.PictureUrl = newPictureUrl;

            await _uow.SaveChangesAsync(ct);
        }
        catch
        {
            if (!string.IsNullOrWhiteSpace(newPictureUrl))
            {
                try
                {
                    await _files.DeleteAsync(newPictureUrl, ct);
                }
                catch
                {
                    // ignored
                }
            }

            throw;
        }

        if (!string.IsNullOrWhiteSpace(oldPictureUrl) &&
            !string.Equals(oldPictureUrl, newPictureUrl, StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                await _files.DeleteAsync(oldPictureUrl, ct);
            }
            catch
            {
                // ignored
            }
        }

        return newPictureUrl;
    }
}