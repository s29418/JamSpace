import React, { useEffect, useMemo, useState } from "react";
import { useChatHub } from "features/chat/model/useChatHub";
import { useInbox } from "features/chat/model/useInbox";
import { Navigate } from "react-router-dom";
import { getToken } from "shared/lib/utils/auth";
import ConversationList from "./ConversationList/ConversationList";
import ConversationPlaceholder from "./ConversationPlaceholder/ConversationPlaceholder";
import styles from "./ChatPage.module.css";

const ChatPage = () => {
    const token = getToken();

    useChatHub();

    const {
        conversations,
        loading,
        error,
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

    if (!token) {
        return <Navigate to="/profile" replace />;
    }

    // @ts-ignore
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
                    <section className={styles.previewPanel}>
                        <div className={styles.previewHeader}>
                            <div className={styles.avatarWrapper}>
                                {activeConversation.displayPictureUrl ? (
                                    <img
                                        src={activeConversation.displayPictureUrl}
                                        alt={activeConversation.displayName}
                                        className={styles.avatar}
                                    />
                                ) : (
                                    <div className={styles.avatarFallback}>
                                        {activeConversation.displayName.charAt(0).toUpperCase()}
                                    </div>
                                )}
                            </div>
                            {activeConversation.displayName}
                        </div>

                        <div className={styles.previewBody}>
                            ...
                        </div>
                    </section>
                ) : (
                    <ConversationPlaceholder />
                )}
            </div>
        </div>
    );
};

export default ChatPage;