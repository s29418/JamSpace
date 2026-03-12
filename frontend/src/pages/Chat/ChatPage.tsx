import React, { useEffect, useMemo, useState } from "react";
import { useSearchParams } from "react-router-dom";
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
        updateConversationPreview,
    } = useInbox();

    const [activeConversationId, setActiveConversationId] = useState<string | null>(null);

    const [searchParams] = useSearchParams();
    const requestedConversationId = searchParams.get("conversationId");

    useEffect(() => {
        if (requestedConversationId && conversations.some((x) => x.id === requestedConversationId)) {
            setActiveConversationId(requestedConversationId);
            return;
        }

        if (!activeConversationId && conversations.length > 0) {
            setActiveConversationId(conversations[0].id);
        }
    }, [requestedConversationId, conversations, activeConversationId]);

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
                        updateConversationPreview={updateConversationPreview}
                    />
                ) : (
                    <ConversationPlaceholder />
                )}
            </div>
        </div>
    );
};

export default ChatPage;