import { useEffect, useMemo, useState } from "react";
import { getConversationDetails } from "entities/chat/api/conversations.api";
import type { ConversationDetailsDto } from "entities/chat/model/types";
import { ApiError, isApiError } from "shared/lib/api/base";

export function useConversationDetails(conversationId: string | null) {
    const [details, setDetails] = useState<ConversationDetailsDto | null>(null);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        setDetails(null);
        setError(null);

        if (!conversationId) {
            setLoading(false);
            return;
        }

        let cancelled = false;

        const load = async () => {
            setLoading(true);
            setError(null);

            try {
                const data = await getConversationDetails(conversationId);
                if (!cancelled) {
                    setDetails(data);
                }
            } catch (e) {
                if (cancelled) return;

                const msg = isApiError(e)
                    ? (e as ApiError).message
                    : "Failed to load conversation details";

                setError(msg);
            } finally {
                if (!cancelled) {
                    setLoading(false);
                }
            }
        };

        void load();

        return () => {
            cancelled = true;
        };
    }, [conversationId]);

    const participantsMap = useMemo(() => {
        const map = new Map<
            string,
            { displayName: string; avatarUrl: string | null; lastReadMessageId: string | null; lastReadAt: string | null }
        >();

        details?.participants.forEach((participant) => {
            map.set(participant.userId, {
                displayName: participant.displayName,
                avatarUrl: participant.avatarUrl,
                lastReadMessageId: participant.lastReadMessageId,
                lastReadAt: participant.lastReadAt,
            });
        });

        return map;
    }, [details]);

    return {
        details,
        participantsMap,
        loading,
        error,
    };
}