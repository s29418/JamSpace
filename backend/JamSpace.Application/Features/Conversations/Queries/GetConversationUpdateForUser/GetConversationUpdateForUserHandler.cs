using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Features.Conversations.DTOs;
using MediatR;

namespace JamSpace.Application.Features.Conversations.Queries.GetConversationUpdateForUser;

public class GetConversationUpdateForUserHandler : IRequestHandler<GetConversationUpdateForUserQuery, ConversationUpdatedDto>
{
    private readonly IConversationParticipantRepository _conversationParticipant;
    private readonly IConversationRepository _conversation;
    private readonly IMessageRepository _message;
    
    public async Task<ConversationUpdatedDto> Handle(GetConversationUpdateForUserQuery request, CancellationToken ct)
    {
        var conversation = await _conversation.GetByIdAsync(request.ConversationId, ct);
        if (conversation is null)
            throw new NotFoundException("Conversation not found.");
        
        var isParticipant = await _conversationParticipant.IsParticipantAsync(request.ConversationId, request.UserId, ct);
        if (!isParticipant)
            throw new ForbiddenAccessException("You are not part of this conversation");

        var lastReadAt = await _conversationParticipant.GetLastReadAt(request.ConversationId, 
            request.UserId, ct);

        var unread = await _message.CountUnreadAsync(request.ConversationId, request.UserId, lastReadAt, ct);

        return new ConversationUpdatedDto
        {
            ConversationId = conversation.Id,
            LastMessageAt = conversation.LastMessageAt,
            LastMessagePreview = conversation.LastMessagePreview,
            UnreadCount = unread
        };
    }
}