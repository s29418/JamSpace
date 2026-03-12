import { useEffect } from "react";
import { chatHub } from "shared/lib/realtime/chatHub";
import { getCurrentUserId } from "shared/lib/utils/auth";

export function useChatHub() {
    const currentUserId = getCurrentUserId();

    useEffect(() => {
        if (!currentUserId) return;

        void chatHub.start();
    }, [currentUserId]);
}