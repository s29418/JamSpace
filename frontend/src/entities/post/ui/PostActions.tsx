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
    onToggleLike?: (post: Post) => void | Promise<void>;
    onToggleRepost?: (post: Post) => void | Promise<void>;
};

export const PostActions: React.FC<Props> = ({
    post,
    showRepostAction,
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
                className={`${styles.actionButton} ${post.isLikedByCurrentUser ? styles.actionActive : ''}`}
                aria-label="Likes"
                onClick={() => void handleLike()}
                disabled={likeBusy}
            >
                <HeartIcon />
                <span className={styles.actionValue}>{post.likeCount}</span>
            </button>

            <button type="button" className={styles.actionButton} aria-label="Comments">
                <ChatBubbleOvalLeftIcon />
                <span className={styles.actionValue}>{post.commentCount}</span>
            </button>

            {showRepostAction && (
                <button
                    type="button"
                    className={`${styles.actionButton} ${post.isRepostedByCurrentUser ? styles.actionActive : ''}`}
                    aria-label="Reposts"
                    onClick={() => void handleRepost()}
                    disabled={repostBusy}
                >
                    <ArrowPathRoundedSquareIcon />
                    <span className={styles.actionValue}>{post.repostCount}</span>
                </button>
            )}
        </div>
    );
};
