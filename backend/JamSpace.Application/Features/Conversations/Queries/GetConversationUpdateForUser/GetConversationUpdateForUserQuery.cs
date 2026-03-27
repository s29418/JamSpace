using JamSpace.Application.Features.Conversations.DTOs;
using MediatR;

namespace JamSpace.Application.Features.Conversations.Queries.GetConversationUpdateForUser;

public record GetConversationUpdateForUserQuery(Guid ConversationId, Guid UserId) : IRequest<ConversationUpdatedDto>;