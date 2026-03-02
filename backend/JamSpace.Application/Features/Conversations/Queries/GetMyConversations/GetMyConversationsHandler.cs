using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Features.Conversations.DTOs;
using JamSpace.Application.Features.Conversations.Strategies;
using MediatR;

namespace JamSpace.Application.Features.Conversations.Queries.GetMyConversations;

public class GetMyConversationsHandler :  IRequestHandler<GetMyConversationsQuery, IEnumerable<ConversationCardDto>>
{
    private readonly IConversationRepository _repo;
    private readonly IConversationCardStrategyResolver _resolver;

    public GetMyConversationsHandler(IConversationRepository repo, IConversationCardStrategyResolver resolver)
    {
        _repo = repo;
        _resolver = resolver;
    } 
    
    public async Task<IEnumerable<ConversationCardDto>> Handle(GetMyConversationsQuery request, CancellationToken cancellationToken)
    {
        var conversations = await _repo.GetForUserAsync(request.UserId, cancellationToken);

        return conversations
            .Select(c => _resolver.Resolve(c.Type).Map(c, request.UserId))
            .ToList();
    }
}