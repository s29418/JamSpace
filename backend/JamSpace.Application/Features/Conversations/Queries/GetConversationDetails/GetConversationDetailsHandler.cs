using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Features.Conversations.DTOs;
using JamSpace.Domain.Entities;
using JamSpace.Domain.Enums;
using MediatR;

namespace JamSpace.Application.Features.Conversations.Queries.GetConversationDetails;

public class GetConversationDetailsHandler : IRequestHandler<GetConversationDetailsQuery, ConversationDetailsDto>
{
    private readonly IConversationRepository _conversation;
    
    public GetConversationDetailsHandler(IConversationRepository conversation)
    {
        _conversation = conversation;
    }

    public async Task<ConversationDetailsDto> Handle(GetConversationDetailsQuery request, CancellationToken ct)
    {
        var conversation = await _conversation.GetByIdAsync(request.Id, ct);

        if (conversation is null)
            throw new NotFoundException($"Conversation with id {request.Id} was not found.");

        var participants = conversation.Participants.ToList();

        if (participants.Count < 2)
            throw new InvalidOperationException("Conversation must have at least two participants.");

        if (!participants.Any(p => p.UserId == request.UserId))
            throw new ForbiddenAccessException("User is not a participant of this conversation.");

        if (conversation.Type == ChatType.Direct && participants.Count != 2)
            throw new InvalidOperationException("Direct conversation must have exactly two participants.");

        var participantDtos = participants
            .Select(p => new ConversationParticipantDto
            {
                UserId = p.UserId,
                DisplayName = p.User!.DisplayName,
                AvatarUrl = p.User.ProfilePictureUrl,
                LastReadMessageId = p.LastReadMessageId,
                LastReadAt = p.LastReadAt
            })
            .ToList();

        var displayData = ResolveDisplayData(conversation, participants, request.UserId);

        return new ConversationDetailsDto
        {
            Id = conversation.Id,
            Type = conversation.Type,
            TargetUserId = displayData.TargetUserId,
            TeamId = conversation.TeamId,
            DisplayName = displayData.DisplayName,
            DisplayPictureUrl = displayData.DisplayPictureUrl,
            Participants = participantDtos
        };
    }

    private static (Guid? TargetUserId, string DisplayName, string? DisplayPictureUrl) ResolveDisplayData(
        Conversation conversation,
        List<ConversationParticipant> participants,
        Guid currentUserId)
    {
        return conversation.Type switch
        {
            ChatType.Direct =>
                ResolveDirectDisplayData(participants, currentUserId),

            ChatType.Team => (
                null,
                conversation.Team?.Name
                ?? throw new InvalidOperationException("Team conversation must have Team loaded."),
                conversation.Team.TeamPictureUrl
            ),

            _ => throw new InvalidOperationException($"Unsupported chat type: {conversation.Type}")
        };
    }
    
    private static (Guid? TargetUserId, string DisplayName, string? DisplayPictureUrl)
        ResolveDirectDisplayData(List<ConversationParticipant> participants, Guid currentUserId)
    {
        var other = participants.First(p => p.UserId != currentUserId);

        return (
            other.UserId,
            other.User!.DisplayName,
            other.User.ProfilePictureUrl
        );
    }
}