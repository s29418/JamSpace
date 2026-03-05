using JamSpace.Application.Common.Interfaces;
using MediatR;

namespace JamSpace.Application.Features.Conversations.Queries.GetConversationAccess;

public class GetConversationAccessHandler : IRequestHandler<GetConversationAccessQuery, bool>
{
    private readonly IConversationParticipantRepository _conversationParticipant;

    public GetConversationAccessHandler(IConversationParticipantRepository conversationParticipant)
    {
        _conversationParticipant = conversationParticipant;
    }

    public async Task<bool> Handle(GetConversationAccessQuery request, CancellationToken cancellationToken)
    {
        return await _conversationParticipant.IsParticipantAsync(request.ConversationId, request.UserId, cancellationToken);
    }
}