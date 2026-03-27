namespace JamSpace.API.Hubs.Contracts;

public sealed record MarkReadRequest(Guid ConversationId, Guid? LastReadMessageId);