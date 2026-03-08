import { useEffect } from "react";
import { chatHub } from "shared/lib/realtime/chatHub";
import { getToken } from "shared/lib/utils/auth";

export function useChatHub() {
    useEffect(() => {
        const token = getToken();
        if (!token) return;

        void chatHub.start();
    }, []);
}