import React, { useMemo } from 'react';
import { useInbox } from 'features/chat/model/useInbox';
import ConversationView from 'widgets/chat-layout/ui/ConversationView';
import styles from './TeamChat.module.css';

type Props = {
    teamId: string;
};

const TeamChat = ({ teamId }: Props) => {
    const {
        conversations,
        loading,
        error,
        markConversationLocallyAsRead,
        updateConversationPreview,
    } = useInbox();

    const teamConversation = useMemo(
        () => conversations.find((conversation) => conversation.type === 'Team' && conversation.teamId === teamId) ?? null,
        [conversations, teamId]
    );

    return (
        <section className={styles.section}>

            <div className={styles.chatSurface}>
                {loading && !teamConversation && <p className={styles.note}>Loading team chat...</p>}
                {!loading && error && !teamConversation && <p className={styles.error}>{error}</p>}
                {!loading && !error && !teamConversation && (
                    <p className={styles.note}>This team chat is not available yet.</p>
                )}

                {teamConversation && (
                    <ConversationView
                        key={teamConversation.id}
                        conversation={teamConversation}
                        onMarkedAsRead={markConversationLocallyAsRead}
                        updateConversationPreview={updateConversationPreview}
                    />
                )}
            </div>
        </section>
    );
};

export default TeamChat;
