import React from "react";
import { useChatHub } from "features/chat/model/useChatHub";

const ChatPage = () => {
    useChatHub();

    return (
        <div>
            Chat page
        </div>
    );
};

export default ChatPage;