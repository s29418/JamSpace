import { useCallback, useEffect, useMemo, useState } from "react";
import { getConversationMessages } from "entities/chat/api/conversations.api";
import type { MessageDto } from "entities/chat/model/types";
import { ApiError, isApiError } from "shared/lib/api/base";
import { chatHub } from "shared/lib/realtime/chatHub";
import { getCurrentUserId } from "shared/lib/utils/auth";

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
    const currentUserId = getCurrentUserId();

    const [messages, setMessages] = useState<MessageDto[]>([]);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);

    const lastMessageId = useMemo(
        () => (messages.length > 0 ? messages[messages.length - 1].id : null),
        [messages]
    );

    const refresh = useCallback(async () => {
        if (!conversationId) {
            setMessages([]);
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
        }finally {
            setLoading(false);
        }
    }, [conversationId, onMarkedAsRead]);

    useEffect(() => {
        if (!conversationId) {
            setMessages([]);
            return;
        }

        let isCancelled = false;

        const init = async () => {
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
    }, [conversationId, refresh]);

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

    const appendOwnMessage = useCallback((message: MessageDto) => {
        setMessages((prev) => {
            if (prev.some((x) => x.id === message.id)) return prev;
            return sortMessages([...prev, message]);
        });
    }, []);

    return {
        messages,
        loading,
        error,
        refresh,
        lastMessageId,
        appendOwnMessage,
    };
}