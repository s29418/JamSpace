import React from "react";
import styles from "./ConversationList.module.css";
import type { ConversationListItemDto } from "entities/chat/model/types";
import ConversationListItem from "../ConversationListItem/ConversationListItem";

type Props = {
    conversations: ConversationListItemDto[];
    activeConversationId: string | null;
    loading: boolean;
    error: string | null;
    onSelectConversation: (conversationId: string) => void;
};

const ConversationList = ({
                              conversations,
                              activeConversationId,
                              loading,
                              error,
                              onSelectConversation,
                          }: Props) => {
    return (
        <aside className={styles.sidebar}>
            <h2 className={styles.title}>Messages</h2>

            {loading && <div className={styles.info}>Loading conversations...</div>}
            {error && <div className={styles.error}>{error}</div>}

            {!loading && !error && conversations.length === 0 && (
                <div className={styles.info}>No conversations yet</div>
            )}

            <div className={styles.scrollContainer}>
                <div className={styles.list}>
                    {conversations.map((conversation) => (
                        <ConversationListItem
                            key={conversation.id}
                            conversation={conversation}
                            isActive={conversation.id === activeConversationId}
                            onClick={onSelectConversation}
                        />
                    ))}
                </div>
            </div>
        </aside>
    );
};

export default ConversationList;