using JamSpace.Application.Features.Conversations.DTOs;
using MediatR;

namespace JamSpace.Application.Features.Conversations.Queries.GetMyConversations;

public record GetMyConversationsQuery(Guid UserId) : IRequest<IEnumerable<ConversationCardDto>>;