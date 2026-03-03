using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Features.Conversations.DTOs;
using JamSpace.Application.Features.Conversations.Strategies;
using MediatR;

namespace JamSpace.Application.Features.Conversations.Queries.GetMyConversations;

public class GetMyConversationsHandler :  IRequestHandler<GetMyConversationsQuery, IReadOnlyList<ConversationCardDto>>
{
    private readonly IConversationRepository _repo;
    private readonly IMessageRepository _message;
    private readonly IConversationCardStrategyResolver _resolver;

    public GetMyConversationsHandler(IConversationRepository repo, IConversationCardStrategyResolver resolver, 
        IMessageRepository message)
    {
        _repo = repo;
        _resolver = resolver;
        _message = message;
    } 
    
    public async Task<IReadOnlyList<ConversationCardDto>> Handle(GetMyConversationsQuery request, CancellationToken ct)
    {
        var conversations = await _repo.GetForUserAsync(request.UserId, ct);
        if (conversations.Count == 0)
            return Array.Empty<ConversationCardDto>();

        var conversationIds = conversations.Select(c => c.Id).ToList();

        var unreadMap = await _message.GetUnreadCountsAsync(request.UserId, conversationIds, ct);

        return conversations
            .Select(c =>
            {
                var unread = unreadMap.GetValueOrDefault(c.Id);
                return _resolver.Resolve(c.Type).Map(c, request.UserId, unread);
            })
            .ToList();
    }
}