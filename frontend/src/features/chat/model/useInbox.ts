import { useCallback, useEffect, useState } from "react";
import { getConversations } from "entities/chat/api/conversations.api";
import type {
    ConversationListItemDto,
    ConversationUpdatedEvent,
} from "entities/chat/model/types";
import { ApiError, isApiError } from "shared/lib/api/base";
import { chatHub } from "shared/lib/realtime/chatHub";

function sortConversations(items: ConversationListItemDto[]) {
    return [...items].sort((a, b) => {
        const aTime = a.lastMessageAt ? new Date(a.lastMessageAt).getTime() : 0;
        const bTime = b.lastMessageAt ? new Date(b.lastMessageAt).getTime() : 0;
        return bTime - aTime;
    });
}

export function useInbox() {
    const [conversations, setConversations] = useState<ConversationListItemDto[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    const refresh = useCallback(async () => {
        setLoading(true);
        setError(null);

        try {
            const data = await getConversations();
            setConversations(sortConversations(data));
        } catch (e) {
            const msg = isApiError(e) ? (e as ApiError).message : "Failed to load conversations";
            setError(msg);
        } finally {
            setLoading(false);
        }
    }, []);

    useEffect(() => {
        void refresh();
    }, [refresh]);

    useEffect(() => {
        const unsubscribe = chatHub.onConversationUpdated((event: ConversationUpdatedEvent) => {
            setConversations((prev) => {
                const existing = prev.find((x) => x.id === event.conversationId);
                if (!existing) return prev;

                const updated: ConversationListItemDto = {
                    ...existing,
                    lastMessageAt: event.lastMessageAt,
                    lastMessagePreview: event.lastMessagePreview,
                    unreadCount: event.unreadCount,
                };

                const filtered = prev.filter((x) => x.id !== event.conversationId);
                return [updated, ...filtered];
            });
        });

        return unsubscribe;
    }, []);

    const markConversationLocallyAsRead = useCallback((conversationId: string) => {
        setConversations((prev) =>
            prev.map((item) =>
                item.id === conversationId
                    ? { ...item, unreadCount: 0 }
                    : item
            )
        );
    }, []);

    return {
        conversations,
        setConversations,
        loading,
        error,
        refresh,
        markConversationLocallyAsRead,
    };
}