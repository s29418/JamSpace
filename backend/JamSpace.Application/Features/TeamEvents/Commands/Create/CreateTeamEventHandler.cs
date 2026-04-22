using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Common.Persistence;
using JamSpace.Application.Features.TeamEvents.DTOs;
using JamSpace.Application.Features.TeamEvents.Mappers;
using JamSpace.Domain.Entities;
using JamSpace.Domain.Enums;
using MediatR;

namespace JamSpace.Application.Features.TeamEvents.Commands.Create;

public class CreateTeamEventHandler : IRequestHandler<CreateTeamEventCommand, TeamEventDto>
{
    private readonly ITeamEventRepository _events;
    private readonly ITeamMemberRepository _members;
    private readonly IUnitOfWork _uow;

    public CreateTeamEventHandler(ITeamEventRepository events, ITeamMemberRepository members, IUnitOfWork uow)
    {
        _events = events;
        _members = members;
        _uow = uow;
    }

    public async Task<TeamEventDto> Handle(CreateTeamEventCommand request, CancellationToken ct)
    {
        var participant =
            await _members.HasRequiredRoleAsync(request.TeamId, request.RequestingUserId, FunctionalRole.Member, ct);

        if (!participant)
            throw new ForbiddenAccessException("You are not part of this team.");

        var eventId = Guid.NewGuid();
        var teamEvent = new TeamEvent
        {
            Id = eventId,
            TeamId = request.TeamId,
            CreatedById = request.RequestingUserId,
            Title = request.Title,
            Description = request.Description,
            StartDateTime = request.StartDateTime,
            DurationMinutes = request.DurationMinutes,
            CreatedAt = DateTimeOffset.UtcNow
        };

        await _events.AddAsync(teamEvent, ct);
        await _uow.SaveChangesAsync(ct);

        var teamEventDb = await _events.GetById(eventId, ct);

        return TeamEventMapper.ToDto(teamEventDb!);
    }
}