using FluentValidation;
using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Common.Persistence;
using JamSpace.Application.Features.Conversations.DTOs;
using JamSpace.Domain.Entities;
using MediatR;

namespace JamSpace.Application.Features.Conversations.Commands.SendMessage;

public class SendMessageHandler : IRequestHandler<SendMessageCommand, MessageDto>
{
    private readonly IConversationRepository _conversation;
    private readonly IConversationParticipantRepository _conversationParticipant;
    private readonly IMessageRepository _message;
    private readonly IUnitOfWork _uow;


    public SendMessageHandler(IConversationRepository conversation, 
        IConversationParticipantRepository conversationParticipant, IMessageRepository message, IUnitOfWork uow)
    {
        _conversation = conversation;
        _conversationParticipant = conversationParticipant;
        _message = message;
        _uow = uow;
    }

    public async Task<MessageDto> Handle(SendMessageCommand request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Content))
            throw new ValidationException("Message content cannot be empty");

        var isParticipant =
            await _conversationParticipant.IsParticipantAsync(request.ConversationId, request.SenderUserId, ct);
        if (!isParticipant)
            throw new ForbiddenAccessException("You are not part of this conversation.");

        var conversation = await _conversation.GetByIdAsync(request.ConversationId, ct);
        if (conversation is null)
            throw new NotFoundException("Conversation not found");

        var message = new Message
        {
            Id = Guid.NewGuid(),
            ConversationId = request.ConversationId,
            SenderUserId = request.SenderUserId,
            Content = request.Content.Trim(),
            CreatedAt = DateTimeOffset.UtcNow,
            ReplyToMessageId = request.ReplyToMessageId
        };

        await _message.AddAsync(message, ct);

        conversation.LastMessageId = message.Id;
        conversation.LastMessageAt = message.CreatedAt;
        conversation.LastMessagePreview = BuildPreview(message.Content);

        await _uow.SaveChangesAsync(ct);

        return new MessageDto
        {
            Id = message.Id,
            ConversationId = message.ConversationId,
            SenderUserId = message.SenderUserId,
            Content = message.Content,
            CreatedAt = message.CreatedAt,
            ReplyToMessageId = message.ReplyToMessageId
        };
    }
    
    private static string BuildPreview(string content)
        => content.Length <= 120 ? content : content[..120];
}