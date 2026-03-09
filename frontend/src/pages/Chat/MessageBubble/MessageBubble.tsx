import React from "react";
import type { MessageDto } from "entities/chat/model/types";
import styles from "./MessageBubble.module.css";

type Props = {
    message: MessageDto;
    isOwn: boolean;
    showAvatar: boolean;
};

function formatMessageTime(value: string) {
    return new Date(value).toLocaleTimeString([], {
        hour: "2-digit",
        minute: "2-digit",
    });
}

const MessageBubble = ({ message, isOwn, showAvatar }: Props) => {
    return (
        <div className={`${styles.row} ${isOwn ? styles.ownRow : styles.otherRow}`}>
            {!isOwn && (
                <div className={styles.avatarContainer}>
                    {showAvatar ? (
                        <img
                            src={message.senderPictureUrl}
                            className={styles.avatar}
                            alt=""
                        />
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
    );
};

export default MessageBubble;