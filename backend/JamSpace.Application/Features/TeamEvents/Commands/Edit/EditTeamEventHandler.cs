using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Common.Persistence;
using JamSpace.Application.Features.TeamEvents.DTOs;
using JamSpace.Application.Features.TeamEvents.Mappers;
using JamSpace.Domain.Enums;
using MediatR;

namespace JamSpace.Application.Features.TeamEvents.Commands.Edit;

public class EditTeamEventHandler : IRequestHandler<EditTeamEventCommand, TeamEventDto>
{
    private readonly ITeamEventRepository _events;
    private readonly ITeamMemberRepository _members;
    private readonly IUnitOfWork _uow;

    public EditTeamEventHandler(ITeamEventRepository events, ITeamMemberRepository members, IUnitOfWork uow)
    {
        _events = events;
        _members = members;
        _uow = uow;
    }

    public async Task<TeamEventDto> Handle(EditTeamEventCommand request, CancellationToken ct)
    {
        var teamEvent = await _events.GetByIdAsync(request.EventId, ct);
        if (teamEvent is null || teamEvent.TeamId != request.TeamId)
            throw new NotFoundException("Event not found.");
        
        var isParticipant =
            await _members.HasRequiredRoleAsync(request.TeamId, request.RequestingUserId, FunctionalRole.Member, ct);
        if (!isParticipant)
            throw new ForbiddenAccessException("You are not part of this team.");

        var canEdit = await _events.WasEventCreatedByUserAsync(request.EventId, request.RequestingUserId, ct);
        if (!canEdit)
            throw new ForbiddenAccessException("You can only edit events created by yourself.");

        if (!string.IsNullOrWhiteSpace(request.Title))
            teamEvent.Title = request.Title;

        if (request.Description is not null)
            teamEvent.Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description;

        if (request.StartDateTime is not null)
            teamEvent.StartDateTime = (DateTimeOffset)request.StartDateTime;

        if (request.DurationMinutes is not null)
            teamEvent.DurationMinutes = (int)request.DurationMinutes;

        await _uow.SaveChangesAsync(ct);

        var teamEventDb = await _events.GetByIdAsync(teamEvent.Id, ct);
        return TeamEventMapper.ToDto(teamEventDb!);
    }
}