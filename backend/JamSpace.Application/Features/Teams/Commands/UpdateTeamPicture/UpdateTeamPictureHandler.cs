using JamSpace.Application.Common.Enums;
using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Common.Persistence;
using JamSpace.Domain.Enums;
using MediatR;

namespace JamSpace.Application.Features.Teams.Commands.UpdateTeamPicture;

public sealed class UpdateTeamPictureHandler : IRequestHandler<UpdateTeamPictureCommand, string>
{
    private readonly ITeamRepository _teams;
    private readonly ITeamMemberRepository _members;
    private readonly IUnitOfWork _uow;
    private readonly IFileStorageService _fileStorageService;

    public UpdateTeamPictureHandler(
        ITeamRepository teams,
        ITeamMemberRepository members,
        IUnitOfWork uow,
        IFileStorageService fileStorageService)
    {
        _teams = teams;
        _members = members;
        _uow = uow;
        _fileStorageService = fileStorageService;
    }

    public async Task<string> Handle(UpdateTeamPictureCommand c, CancellationToken ct)
    {
        var team = await _teams.GetByIdAsync(c.TeamId, ct)
                   ?? throw new NotFoundException("Team not found.");

        var canEdit = await _members.HasRequiredRoleAsync(c.TeamId, c.RequestingUserId, FunctionalRole.Admin, ct);
        if (!canEdit)
            throw new ForbiddenAccessException("Only team leader or admin can update team picture.");

        var oldPictureUrl = team.TeamPictureUrl;
        string? newPictureUrl = null;

        try
        {
            newPictureUrl = await _fileStorageService.UploadAsync(
                c.File,
                StorageObjectType.TeamPicture,
                c.TeamId,
                ct);

            team.TeamPictureUrl = newPictureUrl;

            await _uow.SaveChangesAsync(ct);
        }
        catch
        {
            if (!string.IsNullOrWhiteSpace(newPictureUrl))
            {
                try
                {
                    await _fileStorageService.DeleteAsync(newPictureUrl, ct);
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
                await _fileStorageService.DeleteAsync(oldPictureUrl, ct);
            }
            catch
            {
                // ignored
            }
        }

        return newPictureUrl;
    }
}