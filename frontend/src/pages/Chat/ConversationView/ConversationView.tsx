import React, { useEffect, useLayoutEffect, useMemo, useRef } from "react";
import { useNavigate } from "react-router-dom";
import type { ConversationListItemDto, MessageDto } from "entities/chat/model/types";
import { useConversationMessages } from "features/chat/model/useConversationMessages";
import { useConversationDetails } from "features/chat/model/useConversationDetails";
import { useTyping } from "features/chat/model/useTyping";
import { chatHub } from "shared/lib/realtime/chatHub";
import { getCurrentUserId } from "shared/lib/utils/auth";
import MessageBubble from "../MessageBubble/MessageBubble";
import MessageComposer from "../MessageComposer/MessageComposer";
import TypingIndicator from "../TypingIndicator/TypingIndicator";
import DateDivider from "../DateDivider/DateDivider";
import styles from "./ConversationView.module.css";

type Props = {
    conversation: ConversationListItemDto;
    onMarkedAsRead: (conversationId: string) => void;
    updateConversationPreview: (conversationId: string, content: string) => void;
};

type SeenUser = {
    userId: string;
    displayName: string;
    avatarUrl: string | null;
};

const ConversationView = ({ conversation, onMarkedAsRead, updateConversationPreview }: Props) => {
    const currentUserId = getCurrentUserId();
    const navigate = useNavigate();

    const { details, participantsMap } = useConversationDetails(conversation.id);

    const {
        messages,
        loading,
        loadingMore,
        error,
        loadMore,
        hasMore,
        readEvents,
    } = useConversationMessages({
        conversationId: conversation.id,
        onMarkedAsRead,
    });

    const {
        typingUserIds,
        sendTyping,
        stopTypingNow,
    } = useTyping({
        conversationId: conversation.id,
    });

    function shouldShowAvatar(messages: MessageDto[], index: number) {
        const current = messages[index];
        const next = messages[index + 1];

        if (!next) return true;

        return next.senderUserId !== current.senderUserId;
    }

    function isNewDay(messages: MessageDto[], index: number) {
        if (index === 0) return true;

        const current = new Date(messages[index].createdAt);
        const prev = new Date(messages[index - 1].createdAt);

        return (
            current.getFullYear() !== prev.getFullYear() ||
            current.getMonth() !== prev.getMonth() ||
            current.getDate() !== prev.getDate()
        );
    }

    const messagesAreaRef = useRef<HTMLDivElement | null>(null);
    const messagesEndRef = useRef<HTMLDivElement | null>(null);

    const isInitialLoadRef = useRef(true);
    const previousMessagesCountRef = useRef(0);

    useEffect(() => {
        isInitialLoadRef.current = true;
        previousMessagesCountRef.current = 0;
    }, [conversation.id]);

    useLayoutEffect(() => {
        if (loading) return;

        const container = messagesAreaRef.current;
        if (!container) return;

        if (isInitialLoadRef.current) {
            container.scrollTop = container.scrollHeight;

            isInitialLoadRef.current = false;
            previousMessagesCountRef.current = messages.length;
            return;
        }

        if (messages.length > previousMessagesCountRef.current) {
            messagesEndRef.current?.scrollIntoView({
                behavior: "smooth",
                block: "end",
            });
        }

        previousMessagesCountRef.current = messages.length;

    }, [messages.length, loading]);

    const handleHeaderClick = () => {
        if (!details) return;

        if (details.type === "Team" && details.teamId) {
            navigate(`/teams/${details.teamId}`);
            return;
        }

        if (details.type === "Direct" && details.targetUserId) {
            navigate(`/profile/${details.targetUserId}`);
        }
    };

    const typingNames = useMemo(() => {
        return typingUserIds
            .map((userId) => participantsMap.get(userId)?.displayName)
            .filter((name): name is string => !!name);
    }, [typingUserIds, participantsMap]);

    const seenByMessageId = useMemo(() => {
        const map = new Map<string, SeenUser[]>();

        details?.participants
            .filter((participant) => participant.userId !== currentUserId)
            .forEach((participant) => {
                if (!participant.lastReadMessageId) return;

                const existing = map.get(participant.lastReadMessageId) ?? [];
                existing.push({
                    userId: participant.userId,
                    displayName: participant.displayName,
                    avatarUrl: participant.avatarUrl,
                });

                map.set(participant.lastReadMessageId, existing);
            });

        readEvents.forEach((event) => {
            if (!event.lastReadMessageId) return;

            const participant = participantsMap.get(event.userId);
            if (!participant) return;

            map.forEach((users) => {
                const filtered = users.filter((x) => x.userId !== event.userId);
                users.splice(0, users.length, ...filtered);
            });

            const existing = map.get(event.lastReadMessageId) ?? [];
            existing.push({
                userId: event.userId,
                displayName: participant.displayName,
                avatarUrl: participant.avatarUrl,
            });

            map.set(event.lastReadMessageId, existing);
        });

        return map;
    }, [details, readEvents, participantsMap, currentUserId]);

    const handleScroll = async () => {
        const element = messagesAreaRef.current;
        if (!element || !hasMore || loadingMore) return;

        if (element.scrollTop <= 40) {
            const previousHeight = element.scrollHeight;
            await loadMore();

            requestAnimationFrame(() => {
                const newHeight = element.scrollHeight;
                element.scrollTop = newHeight - previousHeight;
            });
        }
    };

    const headerAvatar = useMemo(() => {
        const picture = details?.displayPictureUrl ?? conversation.displayPictureUrl;
        const name = details?.displayName ?? conversation.displayName;

        if (picture) {
            return (
                <img
                    src={picture}
                    alt={name}
                    className={styles.headerAvatar}
                />
            );
        }

        return (
            <div className={styles.headerAvatarFallback}>
                {name.charAt(0).toUpperCase()}
            </div>
        );
    }, [details, conversation.displayPictureUrl, conversation.displayName]);

    const headerName = details?.displayName ?? conversation.displayName;
    const headerType = details?.type ?? conversation.type;

    const handleSendMessage = async (content: string) => {
        await chatHub.sendMessage({
            conversationId: conversation.id,
            content,
            replyToMessageId: null,
        });

        updateConversationPreview(conversation.id, content);
        onMarkedAsRead(conversation.id);
    };

    return (
        <section className={styles.panel}>
            <header className={styles.header} onClick={handleHeaderClick}>
                <div className={styles.headerLeft}>
                    {headerAvatar}
                    <div className={styles.headerText}>
                        <div className={styles.name}>{headerName}</div>
                        <div className={styles.subtext}>{headerType}</div>
                    </div>
                </div>
            </header>

            <div
                ref={messagesAreaRef}
                className={styles.messagesArea}
                onScroll={() => void handleScroll()}
            >
                {loadingMore && <div className={styles.topLoader}>Loading older messages...</div>}
                {loading && <div className={styles.info}>Loading messages...</div>}
                {error && <div className={styles.error}>{error}</div>}

                {!loading && !error && messages.length === 0 && (
                    <div className={styles.info}>No messages yet</div>
                )}

                {!loading && !error && messages.length > 0 && (
                    <div className={styles.messagesList}>
                        {messages.map((message, index) => {
                            const showAvatar = shouldShowAvatar(messages, index);
                            const showDateDivider = isNewDay(messages, index);
                            const sender = participantsMap.get(message.senderUserId);

                            return (
                                <React.Fragment key={message.id}>
                                    {showDateDivider && (
                                        <DateDivider date={message.createdAt} />
                                    )}

                                    <MessageBubble
                                        message={message}
                                        isOwn={message.senderUserId === currentUserId}
                                        showAvatar={showAvatar}
                                        senderAvatarUrl={sender?.avatarUrl ?? null}
                                        senderDisplayName={sender?.displayName ?? null}
                                        seenBy={seenByMessageId.get(message.id) ?? []}
                                    />
                                </React.Fragment>
                            );
                        })}

                        <div ref={messagesEndRef} />
                    </div>
                )}
            </div>

            <div className={styles.typingBar}>
                <TypingIndicator names={typingNames} />
            </div>

            <MessageComposer
                conversationId={conversation.id}
                onSendMessage={handleSendMessage}
                onTyping={() => void sendTyping()}
                onStopTyping={stopTypingNow}
            />
        </section>
    );
};

export default ConversationView;