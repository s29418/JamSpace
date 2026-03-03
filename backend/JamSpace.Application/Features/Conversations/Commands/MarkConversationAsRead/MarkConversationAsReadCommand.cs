using MediatR;

namespace JamSpace.Application.Features.Conversations.Commands.MarkConversationAsRead;

public record MarkConversationAsReadCommand(Guid RequestingUserId, Guid ConversationId) : IRequest<Unit>;