import React from "react";
import styles from "./ConversationListItem.module.css";
import type { ConversationListItemDto } from "entities/chat/model/types";

type Props = {
    conversation: ConversationListItemDto;
    isActive: boolean;
    onClick: (conversationId: string) => void;
};

function formatLastMessageTime(value: string | null) {
    if (!value) return "";

    const date = new Date(value);
    const now = new Date();

    const sameDay = date.toDateString() === now.toDateString();
    if (sameDay) {
        return date.toLocaleTimeString([], {
            hour: "2-digit",
            minute: "2-digit",
        });
    }

    return date.toLocaleDateString();
}

const ConversationListItem = ({ conversation, isActive, onClick }: Props) => {
    return (
        <button
            type="button"
            className={`${styles.card} ${isActive ? styles.active : ""}`}
            onClick={() => onClick(conversation.id)}
        >
            <div className={styles.avatarWrapper}>
                {conversation.displayPictureUrl ? (
                    <img
                        src={conversation.displayPictureUrl}
                        alt={conversation.displayName}
                        className={`${styles.avatar} ${isActive ? styles.activeAvatar : ""}`}
                    />
                ) : (
                    <div className={styles.avatarFallback}>
                        {conversation.displayName.charAt(0).toUpperCase()}
                    </div>
                )}
            </div>

            <div className={styles.content}>
                <div className={styles.topRow}>
                    <span className={styles.name}>{conversation.displayName}</span>
                    {conversation.lastMessageAt && (
                        <span className={styles.time}>
                            {formatLastMessageTime(conversation.lastMessageAt)}
                        </span>
                    )}
                </div>

                <div className={styles.bottomRow}>
                    <span className={styles.preview}>
                        {conversation.lastMessagePreview || "No messages yet"}
                    </span>

                    {conversation.unreadCount > 0 && (
                        <span className={styles.badge}>
                            {conversation.unreadCount}
                        </span>
                    )}
                </div>
            </div>
        </button>
    );
};

export default ConversationListItem;