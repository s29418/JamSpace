import React from "react";
import type { MessageDto } from "entities/chat/model/types";
import SeenAvatars from "../SeenAvatars/SeenAvatars";
import styles from "./MessageBubble.module.css";

type SeenUser = {
    userId: string;
    displayName: string;
    avatarUrl: string | null;
};

type Props = {
    message: MessageDto;
    isOwn: boolean;
    showAvatar: boolean;
    senderAvatarUrl?: string | null;
    senderDisplayName?: string | null;
    seenBy?: SeenUser[];
};

function formatMessageTime(value: string) {
    return new Date(value).toLocaleTimeString([], {
        hour: "2-digit",
        minute: "2-digit",
    });
}

const MessageBubble = ({ message, isOwn, showAvatar, senderAvatarUrl, senderDisplayName, seenBy = [] }: Props) => {
    return (
        <div className={styles.messageBlock}>
            <div className={`${styles.row} ${isOwn ? styles.ownRow : styles.otherRow}`}>
                {!isOwn && (
                    <div className={styles.avatarContainer}>
                        {showAvatar ? (
                            senderAvatarUrl ? (
                                <img
                                    src={senderAvatarUrl}
                                    className={styles.avatar}
                                    alt={senderDisplayName ?? ""}
                                    title={senderDisplayName ?? ""}
                                />
                            ) : (
                                <div
                                    className={styles.avatarFallback}
                                    title={senderDisplayName ?? ""}
                                >
                                    {(senderDisplayName ?? "?").charAt(0).toUpperCase()}
                                </div>
                            )
                        ) : (
                            <div className={styles.avatarSpacer} />
                        )}
                    </div>
                )}

                <div className={`${styles.bubble} ${isOwn ? styles.ownBubble : styles.otherBubble}`}>
                    <div className={styles.content}>{message.content}</div>
                    <div className={styles.time}>
                        {formatMessageTime(message.createdAt)}
                    </div>
                </div>
            </div>

            {isOwn && seenBy.length > 0 && <SeenAvatars users={seenBy} />}
        </div>
    );
};

export default MessageBubble;