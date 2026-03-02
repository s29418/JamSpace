using JamSpace.Domain.Enums;

namespace JamSpace.Application.Features.Conversations.Strategies;

public sealed class ConversationCardStrategyResolver : IConversationCardStrategyResolver
{
    private readonly Dictionary<ChatType, IConversationCardStrategy> _map;

    public ConversationCardStrategyResolver(IEnumerable<IConversationCardStrategy> strategies)
    {
        _map = strategies.ToDictionary(s => s.SupportedType);
    }

    public IConversationCardStrategy Resolve(ChatType type)
    {
        if (_map.TryGetValue(type, out var strategy))
            return strategy;

        throw new NotSupportedException($"No strategy registered for chat type: {type}");
    }
}