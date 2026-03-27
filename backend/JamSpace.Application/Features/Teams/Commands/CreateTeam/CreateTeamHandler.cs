using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Common.Persistence;
using JamSpace.Application.Features.Teams.DTOs;
using JamSpace.Application.Features.Teams.Mappers;
using JamSpace.Domain.Entities;
using JamSpace.Domain.Enums;
using MediatR;

namespace JamSpace.Application.Features.Teams.Commands.CreateTeam;

public sealed class CreateTeamHandler : IRequestHandler<CreateTeamWithUserCommand, TeamDto>
{
    private readonly ITeamRepository _teams;
    private readonly ITeamMemberRepository _members;
    private readonly IConversationRepository _conversation;
    private readonly IConversationParticipantRepository _conversationParticipant;
    private readonly IUnitOfWork _uow;

    public CreateTeamHandler(ITeamRepository teams, ITeamMemberRepository members, IUnitOfWork uow, 
        IConversationParticipantRepository conversationParticipant, IConversationRepository conversation)
    {
        _teams = teams;
        _members = members;
        _uow = uow;
        _conversationParticipant = conversationParticipant;
        _conversation = conversation;
    }

    public async Task<TeamDto> Handle(CreateTeamWithUserCommand request, CancellationToken ct)
    {
        var team = new Team
        {
            Id = Guid.NewGuid(),
            Name = request.Command.Name,
            TeamPictureUrl = request.Command.TeamPictureUrl,
            CreatedById = request.CreatorUserId,
            CreatedAt = DateTime.UtcNow
        };
        await _teams.AddAsync(team, ct);

        var leader = new TeamMember
        {
            TeamId = team.Id,
            UserId = request.CreatorUserId,
            Role = FunctionalRole.Leader
        };
        await _members.AddAsync(leader, ct);
        
        var conversation = new Conversation
        {
            Id = Guid.NewGuid(),
            Type = ChatType.Team,
            TeamId = team.Id
        };
        await _conversation.AddAsync(conversation, ct);

        var conversationParticipant = new ConversationParticipant
        {
            ConversationId = conversation.Id,
            UserId = request.CreatorUserId,
        };
        await _conversationParticipant.AddAsync(conversationParticipant, ct);

        await _uow.SaveChangesAsync(ct);
        
        var created = await _teams.GetByIdAsync(team.Id, ct)
                      ?? throw new NotFoundException("Team not found after creation.");

        return TeamMapper.ToDto(created, request.CreatorUserId);
    }
}