import React from 'react';
import type { Post } from '../../../entities/post/model/types';
import { PostCard } from '../../../entities/post/ui/PostCard';
import styles from './PostFeed.module.css';

type Props = {
    posts: Post[];
    loading: boolean;
    error?: string | null;
    emptyText: string;
    canDelete?: boolean;
    onDeletePost?: (postId: string) => void | Promise<void>;
    onToggleLike?: (post: Post) => void | Promise<void>;
    onToggleRepost?: (post: Post) => void | Promise<void>;
    onAddComment?: (post: Post, content: string) => void | Promise<void>;
    onDeleteComment?: (post: Post, commentId: string) => void | Promise<void>;
};

export const PostFeed: React.FC<Props> = ({
    posts,
    loading,
    error,
    emptyText,
    canDelete = false,
    onDeletePost,
    onToggleLike,
    onToggleRepost,
    onAddComment,
    onDeleteComment,
}) => {
    if (loading) {
        return (
            <section className={styles.section}>
                <div className={styles.message}>Loading posts...</div>
            </section>
        );
    }

    if (error) {
        return (
            <section className={styles.section}>
                <div className={`${styles.message} ${styles.error}`}>{error}</div>
            </section>
        );
    }

    if (!posts.length) {
        return (
            <section className={styles.section}>
                <div className={styles.message}>{emptyText}</div>
            </section>
        );
    }

    return (
        <section className={styles.section}>
            {posts.map((post) => (
                <PostCard
                    key={post.id}
                    post={post}
                    canDelete={canDelete}
                    onDelete={onDeletePost}
                    onToggleLike={onToggleLike}
                    onToggleRepost={onToggleRepost}
                    onAddComment={onAddComment}
                    onDeleteComment={onDeleteComment}
                    enableDetailsNavigation
                    maxVisibleComments={2}
                />
            ))}
        </section>
    );
};
