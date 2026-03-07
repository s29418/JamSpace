using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Common.Persistence;
using JamSpace.Application.Features.Conversations.DTOs;
using MediatR;

namespace JamSpace.Application.Features.Conversations.Commands.MarkConversationAsRead;

public class MarkConversationAsReadHandler : IRequestHandler<MarkConversationAsReadCommand, ConversationReadDto>
{
    private readonly IConversationRepository _conversation;
    private readonly IMessageRepository _message;
    private readonly IUnitOfWork _uow;
    
    public MarkConversationAsReadHandler(IConversationRepository conversation, IUnitOfWork uow, IMessageRepository message)
    {
        _conversation = conversation;
        _uow = uow;
        _message = message;
    }

    public async Task<ConversationReadDto> Handle(MarkConversationAsReadCommand request, CancellationToken ct)
    {
        var conversation = await _conversation.GetByIdAsync(request.ConversationId, ct);

        if (conversation is null)
            throw new NotFoundException($"Conversation with id {request.ConversationId} was not found.");

        var participant = conversation.Participants
            .SingleOrDefault(cp => cp.UserId == request.RequestingUserId);

        if (participant is null)
            throw new ForbiddenAccessException("You are not part of this conversation");

        Guid? targetMessageId;
        DateTimeOffset? targetReadAt;

        if (request.LastReadMessageId is not null)
        {
            var message = await _message.GetByIdAsync(request.LastReadMessageId.Value, ct);

            if (message is null || message.ConversationId != request.ConversationId)
                throw new NotFoundException("Message not found in this conversation.");

            targetMessageId = message.Id;
            targetReadAt = message.CreatedAt;
        }
        else
        {
            targetMessageId = conversation.LastMessageId;
            targetReadAt = conversation.LastMessageAt;
        }
        
        participant.LastReadMessageId = targetMessageId;
        participant.LastReadAt = targetReadAt;

        await _uow.SaveChangesAsync(ct);

        return new ConversationReadDto
        {
            ConversationId = request.ConversationId,
            UserId = request.RequestingUserId,
            LastReadMessageId = participant.LastReadMessageId,
            LastReadAt = participant.LastReadAt
        };
    }
}