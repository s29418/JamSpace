using MediatR;

namespace JamSpace.Application.Features.Conversations.Queries.GetConversationParticipantUserIds;

public record GetConversationParticipantUserIdsQuery(Guid ConversationId) : IRequest<IReadOnlyList<Guid>>;