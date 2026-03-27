using MediatR;

namespace JamSpace.Application.Features.Conversations.Commands.GetOrCreateDirectConversation;

public record GetOrCreateDirectConversationCommand(Guid RequestingUserId, Guid OtherUserId) : IRequest<Guid>;