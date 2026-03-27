using JamSpace.Application.Common.Models;
using JamSpace.Application.Features.Conversations.DTOs;
using MediatR;

namespace JamSpace.Application.Features.Conversations.Queries.GetConversationMessages;

public record GetConversationMessagesQuery(Guid ConversationId, Guid RequestingUserId, 
    DateTimeOffset? Before, int Take) : IRequest<CursorResult<MessageDto>>;