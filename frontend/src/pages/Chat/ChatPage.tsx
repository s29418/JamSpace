import React, { useEffect, useMemo, useState } from "react";
import { useChatHub } from "features/chat/model/useChatHub";
import { useInbox } from "features/chat/model/useInbox";
import ConversationList from "./ConversationList/ConversationList";
import ConversationPlaceholder from "./ConversationPlaceholder/ConversationPlaceholder";
import ConversationView from "./ConversationView/ConversationView";
import styles from "./ChatPage.module.css";

const ChatPage = () => {
    useChatHub();

    const {
        conversations,
        loading,
        error,
        markConversationLocallyAsRead,
    } = useInbox();

    const [activeConversationId, setActiveConversationId] = useState<string | null>(null);

    useEffect(() => {
        if (!activeConversationId && conversations.length > 0) {
            setActiveConversationId(conversations[0].id);
        }
    }, [conversations, activeConversationId]);

    const activeConversation = useMemo(
        () => conversations.find((x) => x.id === activeConversationId) ?? null,
        [conversations, activeConversationId]
    );

    return (
        <div className={styles.page}>
            <div className={styles.layout}>
                <ConversationList
                    conversations={conversations}
                    activeConversationId={activeConversationId}
                    loading={loading}
                    error={error}
                    onSelectConversation={setActiveConversationId}
                />

                {activeConversation ? (
                    <ConversationView
                        conversation={activeConversation}
                        onMarkedAsRead={markConversationLocallyAsRead}
                    />
                ) : (
                    <ConversationPlaceholder />
                )}
            </div>
        </div>
    );
};

export default ChatPage;