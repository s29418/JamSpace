using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Common.Persistence;
using JamSpace.Domain.Enums;
using MediatR;

namespace JamSpace.Application.Features.TeamEvents.Commands.Delete;

public class DeleteTeamEventHandler : IRequestHandler<DeleteTeamEventCommand, Unit>
{
    private readonly ITeamEventRepository _events;
    private readonly ITeamMemberRepository _members;
    private readonly IUnitOfWork _uow;

    public DeleteTeamEventHandler(ITeamEventRepository events, ITeamMemberRepository members, IUnitOfWork uow)
    {
        _events = events;
        _members = members;
        _uow = uow;
    }

    public async Task<Unit> Handle(DeleteTeamEventCommand request, CancellationToken ct)
    {
        var teamEvent = await _events.GetByIdAsync(request.EventId, ct);
        if (teamEvent is null || teamEvent.TeamId != request.TeamId)
            throw new NotFoundException("Event not found.");
        
        var isParticipant =
            await _members.HasRequiredRoleAsync(request.TeamId, request.RequestingUserId, FunctionalRole.Member, ct);
        if (!isParticipant)
            throw new ForbiddenAccessException("You are not part of this team.");

        var canDelete = await _events.WasEventCreatedByUserAsync(request.EventId, request.RequestingUserId, ct) ||
                        await _members.HasRequiredRoleAsync(request.TeamId, request.RequestingUserId,
                            FunctionalRole.Admin, ct);
        if (!canDelete)
            throw new ForbiddenAccessException("You must be an admin to delete events posted by other users.");

        _events.Remove(teamEvent);
        await _uow.SaveChangesAsync(ct);
        
        return Unit.Value;
    }
}