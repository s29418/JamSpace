import React, { useState } from 'react';
import {
    ArrowPathRoundedSquareIcon,
    ChatBubbleOvalLeftIcon,
    HeartIcon,
} from '@heroicons/react/24/outline';
import type { Post } from '../model/types';
import styles from './PostCard.module.css';

type Props = {
    post: Post;
    showRepostAction: boolean;
    onToggleComments?: () => void;
    onToggleLike?: (post: Post) => void | Promise<void>;
    onToggleRepost?: (post: Post) => void | Promise<void>;
};

export const PostActions: React.FC<Props> = ({
    post,
    showRepostAction,
    onToggleComments,
    onToggleLike,
    onToggleRepost,
}) => {
    const [likeBusy, setLikeBusy] = useState(false);
    const [repostBusy, setRepostBusy] = useState(false);

    async function handleLike() {
        if (!onToggleLike || likeBusy) return;

        setLikeBusy(true);
        try {
            await onToggleLike(post);
        } finally {
            setLikeBusy(false);
        }
    }

    async function handleRepost() {
        if (!onToggleRepost || repostBusy) return;

        setRepostBusy(true);
        try {
            await onToggleRepost(post);
        } finally {
            setRepostBusy(false);
        }
    }

    return (
        <div className={styles.actions}>
            <button
                type="button"
                className={styles.actionButton}
                aria-label="Likes"
                onClick={(event) => {
                    event.stopPropagation();
                    void handleLike();
                }}
                disabled={likeBusy}
            >
                <span className={styles.hoverEffect}>
                    <HeartIcon className={`${styles.actionIcon} ${post.isLikedByCurrentUser ? styles.likeIconActive : ''}`}/>
                </span>
                <span className={`${styles.actionValue} ${post.isLikedByCurrentUser ? styles.actionActive : ''}`}>{post.likeCount}</span>
            </button>

            <button
                type="button"
                className={styles.actionButton}
                aria-label="Comments"
                onClick={(event) => {
                    event.stopPropagation();
                    onToggleComments?.();
                }}
            >
                <span className={styles.hoverEffect}>
                    <ChatBubbleOvalLeftIcon className={styles.actionIcon} />
                </span>
                <span className={styles.actionValue}>{post.commentCount}</span>
            </button>

            {showRepostAction && (
                <button
                    type="button"
                    className={styles.repostButton}
                    aria-label="Reposts"
                    onClick={(event) => {
                        event.stopPropagation();
                        void handleRepost();
                    }}
                    disabled={repostBusy}
                >
                    <span className={styles.hoverEffect}>
                        <ArrowPathRoundedSquareIcon className={`${styles.actionIcon} ${post.isRepostedByCurrentUser ? styles.repostActive : ''}`} />
                    </span>
                    <span className={`${styles.actionValue} ${post.isRepostedByCurrentUser ? styles.actionActive : ''}`}>{post.repostCount}</span>
                </button>
            )}
        </div>
    );
};
