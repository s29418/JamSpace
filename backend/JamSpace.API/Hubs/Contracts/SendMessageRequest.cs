namespace JamSpace.API.Hubs.Contracts;

public sealed record SendMessageRequest(Guid ConversationId, string Content, Guid? ReplyToMessageId);