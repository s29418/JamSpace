import { useCallback, useEffect, useMemo, useState } from "react";
import { getConversationMessages } from "entities/chat/api/conversations.api";
import type { ConversationReadEvent, MessageDto } from "entities/chat/model/types";
import { ApiError, isApiError } from "shared/lib/api/base";
import { chatHub } from "shared/lib/realtime/chatHub";
import { useAuthState } from "shared/lib/hooks/useAuthState";
import { waitForAuthReady } from "shared/lib/utils/waitForAuthReady";

type Params = {
    conversationId: string | null;
    onMarkedAsRead?: (conversationId: string) => void;
};

function sortMessages(items: MessageDto[]) {
    return [...items].sort(
        (a, b) => new Date(a.createdAt).getTime() - new Date(b.createdAt).getTime()
    );
}

export function useConversationMessages({ conversationId, onMarkedAsRead }: Params) {
    const { currentUserId, isAuthenticated } = useAuthState();

    const [messages, setMessages] = useState<MessageDto[]>([]);
    const [loading, setLoading] = useState(false);
    const [loadingMore, setLoadingMore] = useState(false);
    const [error, setError] = useState<string | null>(null);
    const [hasMore, setHasMore] = useState(false);
    const [nextBefore, setNextBefore] = useState<string | null>(null);
    const [readEvents, setReadEvents] = useState<ConversationReadEvent[]>([]);

    const lastMessageId = useMemo(
        () => (messages.length > 0 ? messages[messages.length - 1].id : null),
        [messages]
    );

    const refresh = useCallback(async () => {
        if (!conversationId) {
            setMessages([]);
            setReadEvents([]);
            setHasMore(false);
            setNextBefore(null);
            return;
        }

        setLoading(true);
        setError(null);

        try {
            const result = await getConversationMessages({
                conversationId,
                take: 50,
            });

            const sorted = sortMessages(result.items);

            setMessages(sorted);
            setHasMore(result.hasMore);
            setNextBefore(result.nextBefore);
            setReadEvents([]);

            try {
                await chatHub.markRead({
                    conversationId,
                    lastReadMessageId: sorted.length > 0 ? sorted[sorted.length - 1].id : null,
                });

                onMarkedAsRead?.(conversationId);
            } catch (markReadError) {
                console.error("MARK READ ERROR", markReadError);
            }
        } catch (e) {
            const msg = isApiError(e)
                ? (e as ApiError).message
                : "Failed to load conversation messages";

            setError(msg);
        } finally {
            setLoading(false);
        }
    }, [conversationId, onMarkedAsRead]);

    const loadMore = useCallback(async () => {
        if (!conversationId || !hasMore || !nextBefore || loadingMore) return;

        setLoadingMore(true);

        try {
            const result = await getConversationMessages({
                conversationId,
                before: nextBefore,
                take: 30,
            });

            setMessages((prev) => {
                const merged = [...result.items, ...prev];
                const unique = merged.filter(
                    (message, index, arr) => arr.findIndex((x) => x.id === message.id) === index
                );

                return sortMessages(unique);
            });

            setHasMore(result.hasMore);
            setNextBefore(result.nextBefore);
        } catch (e) {
            console.error("Failed to load older messages", e);
        } finally {
            setLoadingMore(false);
        }
    }, [conversationId, hasMore, nextBefore, loadingMore]);

    useEffect(() => {
        if (!conversationId) {
            setMessages([]);
            setReadEvents([]);
            setHasMore(false);
            setNextBefore(null);
            return;
        }

        let isCancelled = false;

        const init = async () => {
            await waitForAuthReady();
            if (isCancelled) return;

            if (!isAuthenticated || !currentUserId) {
                setMessages([]);
                setReadEvents([]);
                setHasMore(false);
                setNextBefore(null);
                return;
            }

            try {
                await chatHub.joinConversation(conversationId);

                if (!isCancelled) {
                    await refresh();
                }
            } catch (e) {
                if (isCancelled) return;

                const msg = isApiError(e)
                    ? (e as ApiError).message
                    : "Failed to open conversation";

                setError(msg);
            }
        };

        void init();

        return () => {
            isCancelled = true;
            void chatHub.leaveConversation(conversationId);
        };
    }, [conversationId, refresh, currentUserId, isAuthenticated]);

    useEffect(() => {
        const unsubscribe = chatHub.onMessageNew(async (message) => {
            if (message.conversationId !== conversationId) return;

            setMessages((prev) => {
                if (prev.some((x) => x.id === message.id)) return prev;
                return sortMessages([...prev, message]);
            });

            if (message.senderUserId !== currentUserId) {
                try {
                    await chatHub.markRead({
                        conversationId: message.conversationId,
                        lastReadMessageId: message.id,
                    });

                    onMarkedAsRead?.(message.conversationId);
                } catch (e) {
                    console.error("Failed to mark message as read", e);
                }
            }
        });

        return unsubscribe;
    }, [conversationId, currentUserId, onMarkedAsRead]);

    useEffect(() => {
        const unsubscribe = chatHub.onConversationRead((event) => {
            if (event.conversationId !== conversationId) return;
            if (event.userId === currentUserId) return;

            setReadEvents((prev) => {
                const filtered = prev.filter((x) => x.userId !== event.userId);
                return [...filtered, event];
            });
        });

        return unsubscribe;
    }, [conversationId, currentUserId]);

    return {
        messages,
        loading,
        loadingMore,
        error,
        refresh,
        loadMore,
        hasMore,
        lastMessageId,
        readEvents,
    };
}