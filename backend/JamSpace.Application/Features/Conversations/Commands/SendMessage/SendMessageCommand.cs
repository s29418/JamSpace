using JamSpace.Application.Features.Conversations.DTOs;
using MediatR;

namespace JamSpace.Application.Features.Conversations.Commands.SendMessage;

public record SendMessageCommand(Guid ConversationId, Guid SenderUserId, string Content, Guid? ReplyToMessageId) 
    : IRequest<MessageDto>;