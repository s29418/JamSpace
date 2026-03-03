using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Common.Persistence;
using MediatR;

namespace JamSpace.Application.Features.Conversations.Commands.MarkConversationAsRead;

public class MarkConversationAsReadHandler : IRequestHandler<MarkConversationAsReadCommand, Unit>
{
    private readonly IConversationRepository _conversation;
    private readonly IUnitOfWork _uow;
    
    public MarkConversationAsReadHandler(IConversationRepository conversation, IUnitOfWork uow)
    {
        _conversation = conversation;
        _uow = uow;
    }

    public async Task<Unit> Handle(MarkConversationAsReadCommand request, CancellationToken ct)
    {
        var conversation = await _conversation.GetByIdAsync(request.ConversationId, ct);

        if (conversation is null)
            throw new NotFoundException($"Conversation with id {request.ConversationId} was not found.");

        var participant = conversation.Participants
            .SingleOrDefault(cp => cp.UserId == request.RequestingUserId);

        if (participant is null)
            throw new NotFoundException("You are not part of this conversation");

        participant.LastReadMessageId = conversation.LastMessageId;

        await _uow.SaveChangesAsync(ct);
        return Unit.Value;
    }
}