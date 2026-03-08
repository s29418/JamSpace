import React, { useEffect, useMemo, useRef } from "react";
import type { ConversationListItemDto } from "entities/chat/model/types";
import { useConversationMessages } from "features/chat/model/useConversationMessages";
import { getCurrentUserId } from "shared/lib/utils/auth";
import MessageBubble from "../MessageBubble/MessageBubble";
import MessageComposer from "../MessageComposer/MessageComposer";
import styles from "./ConversationView.module.css";

type Props = {
    conversation: ConversationListItemDto;
    onMarkedAsRead: (conversationId: string) => void;
};

const ConversationView = ({ conversation, onMarkedAsRead }: Props) => {
    const currentUserId = getCurrentUserId();

    const {
        messages,
        loading,
        error,
    } = useConversationMessages({
        conversationId: conversation.id,
        onMarkedAsRead,
    });

    const messagesEndRef = useRef<HTMLDivElement | null>(null);

    useEffect(() => {
        messagesEndRef.current?.scrollIntoView({ behavior: "smooth" });
    }, [messages.length]);

    const headerAvatar = useMemo(() => {
        if (conversation.displayPictureUrl) {
            return (
                <img
                    src={conversation.displayPictureUrl}
                    alt={conversation.displayName}
                    className={styles.headerAvatar}
                />
            );
        }

        return (
            <div className={styles.headerAvatarFallback}>
                {conversation.displayName.charAt(0).toUpperCase()}
            </div>
        );
    }, [conversation.displayPictureUrl, conversation.displayName]);

    return (
        <section className={styles.panel}>
            <header className={styles.header}>
                <div className={styles.headerLeft}>
                    {headerAvatar}
                    <div className={styles.headerText}>
                        <div className={styles.name}>{conversation.displayName}</div>
                        <div className={styles.subtext}>{conversation.type}</div>
                    </div>
                </div>
            </header>

            <div className={styles.messagesArea}>
                {loading && <div className={styles.info}>Loading messages...</div>}
                {error && <div className={styles.error}>{error}</div>}

                {!loading && !error && messages.length === 0 && (
                    <div className={styles.info}>No messages yet</div>
                )}

                {!loading && !error && messages.length > 0 && (
                    <div className={styles.messagesList}>
                        {messages.map((message) => (
                            <MessageBubble
                                key={message.id}
                                message={message}
                                isOwn={message.senderUserId === currentUserId}
                            />
                        ))}
                        <div ref={messagesEndRef} />
                    </div>
                )}
            </div>

            <MessageComposer conversationId={conversation.id} />
        </section>
    );
};

export default ConversationView;