using JamSpace.Application.Features.Conversations.DTOs;
using MediatR;

namespace JamSpace.Application.Features.Conversations.Commands.MarkConversationAsRead;

public sealed record MarkConversationAsReadCommand(Guid ConversationId, Guid RequestingUserId, Guid? LastReadMessageId) 
    : IRequest<ConversationReadDto>;