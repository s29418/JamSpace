using MediatR;

namespace JamSpace.Application.Features.Conversations.Queries.GetConversationAccess;

public record GetConversationAccessQuery(Guid ConversationId, Guid UserId) : IRequest<bool>;