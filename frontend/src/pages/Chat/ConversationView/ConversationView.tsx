import React, { useEffect, useMemo, useRef } from "react";
import { useNavigate } from "react-router-dom";
import type {ConversationListItemDto, MessageDto} from "entities/chat/model/types";
import { useConversationMessages } from "features/chat/model/useConversationMessages";
import { getCurrentUserId } from "shared/lib/utils/auth";
import MessageBubble from "../MessageBubble/MessageBubble";
import MessageComposer from "../MessageComposer/MessageComposer";
import styles from "./ConversationView.module.css";

type Props = {
    conversation: ConversationListItemDto;
    onMarkedAsRead: (conversationId: string) => void;
    updateConversationPreview: (conversationId: string, content: string) => void;
};

const ConversationView = ({ conversation, onMarkedAsRead, updateConversationPreview }: Props) => {
    const currentUserId = getCurrentUserId();

    const {
        messages,
        loading,
        error,
    } = useConversationMessages({
        conversationId: conversation.id,
        onMarkedAsRead,
    });

    const navigate = useNavigate();
    const handleHeaderClick = () => {
        if (conversation.type === "Team" && conversation.teamId) {
            navigate(`/teams/${conversation.teamId}`);
            return;
        }

        if (conversation.type === "Direct" && conversation.targetUserId) {
            navigate(`/profile/${conversation.targetUserId}`);
        }
    };

    const messagesEndRef = useRef<HTMLDivElement | null>(null);

    useEffect(() => {
        messagesEndRef.current?.scrollIntoView({ behavior: "smooth" });
    }, [messages.length]);

    function shouldShowAvatar(messages: MessageDto[], index: number) {
        const current = messages[index];
        const next = messages[index + 1];

        if (!next) return true;

        return next.senderUserId !== current.senderUserId;
    }

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
            <header className={styles.header} onClick={handleHeaderClick}>
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
                        {messages.map((message, index) => {
                            const showAvatar = shouldShowAvatar(messages, index);

                            return (
                                <MessageBubble
                                    key={message.id}
                                    message={message}
                                    isOwn={message.senderUserId === currentUserId}
                                    showAvatar={showAvatar}
                                />
                            );
                        })}
                        <div ref={messagesEndRef} />
                    </div>
                )}
            </div>

            <MessageComposer
                conversationId={conversation.id}
                onMessageSent={(content) => {
                    updateConversationPreview(conversation.id, content)
                    onMarkedAsRead(conversation.id)
                }}
            />
        </section>
    );
};

export default ConversationView;