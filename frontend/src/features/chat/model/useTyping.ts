import { useCallback, useEffect, useRef, useState } from "react";
import { chatHub } from "shared/lib/realtime/chatHub";
import type { ConversationTypingEvent } from "entities/chat/model/types";
import { getCurrentUserId } from "shared/lib/utils/auth";

type Params = {
    conversationId: string | null;
};

export function useTyping({ conversationId }: Params) {
    const currentUserId = getCurrentUserId();

    const [typingUserIds, setTypingUserIds] = useState<string[]>([]);
    const stopTypingTimeoutRef = useRef<number | null>(null);
    const lastTypingSentRef = useRef(false);

    useEffect(() => {
        const unsubscribe = chatHub.onConversationTyping((event: ConversationTypingEvent) => {
            if (event.conversationId !== conversationId) return;
            if (event.userId === currentUserId) return;

            setTypingUserIds((prev) => {
                if (event.isTyping) {
                    if (prev.includes(event.userId)) return prev;
                    return [...prev, event.userId];
                }

                return prev.filter((id) => id !== event.userId);
            });
        });

        return unsubscribe;
    }, [conversationId, currentUserId]);

    const sendTyping = useCallback(async () => {
        if (!conversationId) return;

        if (!lastTypingSentRef.current) {
            lastTypingSentRef.current = true;
            await chatHub.sendTyping(conversationId, true);
        }

        if (stopTypingTimeoutRef.current) {
            window.clearTimeout(stopTypingTimeoutRef.current);
        }

        stopTypingTimeoutRef.current = window.setTimeout(async () => {
            if (!conversationId) return;

            lastTypingSentRef.current = false;
            await chatHub.sendTyping(conversationId, false);
        }, 2500);
    }, [conversationId]);

    const stopTypingNow = useCallback(async () => {
        if (!conversationId) return;
        if (!lastTypingSentRef.current) return;

        if (stopTypingTimeoutRef.current) {
            window.clearTimeout(stopTypingTimeoutRef.current);
            stopTypingTimeoutRef.current = null;
        }

        lastTypingSentRef.current = false;
        await chatHub.sendTyping(conversationId, false);
    }, [conversationId]);

    useEffect(() => {
        return () => {
            if (stopTypingTimeoutRef.current) {
                window.clearTimeout(stopTypingTimeoutRef.current);
            }
        };
    }, []);

    return {
        typingUserIds,
        sendTyping,
        stopTypingNow,
    };
}