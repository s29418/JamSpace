namespace JamSpace.API.Hubs.Contracts;

public sealed record ConversationTypingDto(Guid ConversationId, Guid UserId, bool IsTyping);