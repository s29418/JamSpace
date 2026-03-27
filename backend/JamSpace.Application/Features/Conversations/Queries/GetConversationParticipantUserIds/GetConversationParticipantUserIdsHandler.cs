using JamSpace.Application.Common.Interfaces;
using MediatR;

namespace JamSpace.Application.Features.Conversations.Queries.GetConversationParticipantUserIds;

public class GetConversationParticipantUserIdsHandler : IRequestHandler<GetConversationParticipantUserIdsQuery, IReadOnlyList<Guid>>
{
    private readonly IConversationParticipantRepository _conversationParticipant;

    public GetConversationParticipantUserIdsHandler(IConversationParticipantRepository conversationParticipant)
    {
        _conversationParticipant = conversationParticipant;
    }
    
    public async Task<IReadOnlyList<Guid>> Handle(GetConversationParticipantUserIdsQuery request, CancellationToken ct)
    {
        return await _conversationParticipant.GetUserIdsAsync(request.ConversationId, ct);
    }
}