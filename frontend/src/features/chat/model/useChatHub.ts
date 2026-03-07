import { useEffect } from "react";
import { chatHub } from "shared/lib/realtime/chatHub";

export function useChatHub() {
    useEffect(() => {
        void chatHub.start();

        return () => {

        };
    }, []);
}