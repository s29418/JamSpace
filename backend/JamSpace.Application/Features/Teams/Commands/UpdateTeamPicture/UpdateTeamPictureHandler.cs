using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Common.Persistence;
using JamSpace.Domain.Enums;
using MediatR;

namespace JamSpace.Application.Features.Teams.Commands.UpdateTeamPicture;

public sealed class UpdateTeamPictureHandler : IRequestHandler<UpdateTeamPictureCommand>
{
    private readonly ITeamRepository _teams;
    private readonly ITeamMemberRepository _members;
    private readonly IUnitOfWork _uow;

    public UpdateTeamPictureHandler(ITeamRepository teams, IUnitOfWork uow, ITeamMemberRepository members)
    {
        _teams = teams;
        _uow = uow;
        _members = members;
    }

    public async Task Handle(UpdateTeamPictureCommand c, CancellationToken ct)
    {
        var team = await _teams.GetByIdAsync(c.TeamId, ct)
                   ?? throw new NotFoundException("Team not found.");

        var canEdit = await _members.HasRequiredRoleAsync(c.TeamId, c.RequestingUserId, FunctionalRole.Admin, ct);
        if (!canEdit)
            throw new ForbiddenAccessException("Only team leader or admin can update team picture.");

        team.TeamPictureUrl = c.PictureUrl;

        await _uow.SaveChangesAsync(ct);
    }
}