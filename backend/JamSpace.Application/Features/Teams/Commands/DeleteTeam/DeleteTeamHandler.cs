using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Common.Persistence;
using JamSpace.Domain.Enums;
using MediatR;

namespace JamSpace.Application.Features.Teams.Commands.DeleteTeam;

public class DeleteTeamHandler : IRequestHandler<DeleteTeamCommand, Unit>
{
    private readonly ITeamRepository _teams;
    private readonly ITeamMemberRepository _teamMembers;
    private readonly IConversationRepository _conversation;
    private readonly IUnitOfWork _uow;

    public DeleteTeamHandler(ITeamRepository teams, ITeamMemberRepository teamMembers, 
        IUnitOfWork uow, IConversationRepository conversation)
    {
        _teams = teams;
        _teamMembers = teamMembers;
        _uow = uow;
        _conversation = conversation;
    }

    public async Task<Unit> Handle(DeleteTeamCommand request, CancellationToken ct)
    {
        if (!await _teamMembers.HasRequiredRoleAsync(request.TeamId, request.RequestingUserId, FunctionalRole.Leader, ct))
            throw new ForbiddenAccessException("Only team leader can delete teams.");

        var team = await _teams.GetByIdAsync(request.TeamId, ct)
                   ?? throw new NotFoundException("Team not found.");

        _teams.Remove(team);

        var conversation = await _conversation.GetForTeam(request.TeamId, ct);
        if (conversation is null)
            throw new NotFoundException($"Conversation for team with ID: {request.TeamId} was not found.");
        
        _conversation.Remove(conversation);

        await _uow.SaveChangesAsync(ct);

        return Unit.Value;
    }
}