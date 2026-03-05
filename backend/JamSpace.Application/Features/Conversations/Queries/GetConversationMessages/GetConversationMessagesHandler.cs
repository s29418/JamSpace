using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Common.Models;
using JamSpace.Application.Features.Conversations.DTOs;
using MediatR;

namespace JamSpace.Application.Features.Conversations.Queries.GetConversationMessages;

public class GetConversationMessagesHandler : IRequestHandler<GetConversationMessagesQuery, CursorResult<MessageDto>>
{
    private readonly IMessageRepository _message;
    private readonly IConversationParticipantRepository _conversationParticipant;

    public GetConversationMessagesHandler(IMessageRepository message, IConversationParticipantRepository conversationParticipant)
    {
        _message = message;
        _conversationParticipant = conversationParticipant;
    }

    public async Task<CursorResult<MessageDto>> Handle(GetConversationMessagesQuery request, CancellationToken ct)
    {
        var isParticipant =
            await _conversationParticipant.IsParticipantAsync(request.ConversationId, request.RequestingUserId, ct);

        if (!isParticipant)
            throw new ForbiddenAccessException("You are not part of this conversation");

        var take = Math.Clamp(request.Take, 1, 100);
        var takePlusOne = take + 1;

        var messages = await _message
            .GetMessagesAsync(request.ConversationId, request.Before, takePlusOne, ct);

        if (messages.Count == 0)
            return CursorResult<MessageDto>.Create(Array.Empty<MessageDto>(), false, null);
        
        var hasMore = messages.Count == takePlusOne;

        var pageMessages = hasMore ? messages.Take(take).ToList() : messages.ToList();
        pageMessages.Reverse();
        
        var nextBefore = pageMessages.First().CreatedAt;

        var dtoItems = pageMessages.Select(m => new MessageDto
        {
            Id = m.Id,
            SenderUserId = m.SenderUserId,
            SenderUsername = m.SenderUser.DisplayName,
            SenderPictureUrl = m.SenderUser.ProfilePictureUrl,
            Content = m.Content,
            CreatedAt = m.CreatedAt,
            ReplyToMessageId = m.ReplyToMessageId
        }).ToList();
        
        return CursorResult<MessageDto>.Create(items: dtoItems, hasMore: hasMore, nextBefore: nextBefore);
    }
}