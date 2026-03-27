using JamSpace.Domain.Enums;

namespace JamSpace.Application.Features.Conversations.Strategies;

public interface IConversationCardStrategyResolver
{
    IConversationCardStrategy Resolve(ChatType type);
}