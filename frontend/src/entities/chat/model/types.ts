export type ConversationType = "Direct" | "Team";

export type ConversationListItemDto = {
    id: string;
    type: ConversationType;
    targetUserId?: string;
    teamId?: string;
    displayName: string;
    displayPictureUrl: string | null;
    lastMessagePreview: string | null;
    lastMessageAt: string | null;
    unreadCount: number;
};

export type MessageDto = {
    id: string;
    conversationId: string;
    senderUserId: string;
    senderPictureUrl: string;
    content: string;
    createdAt: string;
    replyToMessageId: string | null;
};

export type CursorResult<T> = {
    items: T[];
    hasMore: boolean;
    nextBefore: string | null;
};

export type GetMessagesParams = {
    conversationId: string;
    before?: string | null;
    take?: number;
};

export type CreateDirectConversationRequest = {
    otherUserId: string;
};

export type CreateDirectConversationResponse = {
    conversationId: string;
};

export type SendMessagePayload = {
    conversationId: string;
    content: string;
    replyToMessageId: string | null;
};

export type MarkReadPayload = {
    conversationId: string;
    lastReadMessageId: string | null;
};

export type ConversationUpdatedEvent = {
    conversationId: string;
    lastMessageAt: string;
    lastMessagePreview: string | null;
    unreadCount: number;
};

export type ConversationReadEvent = {
    conversationId: string;
    userId: string;
    lastReadMessageId: string | null;
    lastReadAt: string;
};

export type ConversationTypingEvent = {
    conversationId: string;
    userId: string;
    isTyping: boolean;
};