import React from 'react';
import { usePostsFeed } from '../../../features/post/model/usePostsFeed';
import { PostFeed } from '../../../widgets/post-feed/ui/PostFeed';
import styles from './HomePage.module.css';
import { useToast } from '../../../shared/lib/hooks/useToast';
import { isApiError } from '../../../shared/api/base';

const HomePage = () => {
    const { posts, loading, error, toggleLike, toggleRepost, addComment, removeComment } = usePostsFeed({ mode: 'auto' });
    const { message, showError } = useToast();

    async function handleToggleLike(post: Parameters<typeof toggleLike>[0]) {
        try {
            await toggleLike(post);
        } catch (e) {
            showError(isApiError(e) ? e.message : 'Failed to update like.');
        }
    }

    async function handleToggleRepost(post: Parameters<typeof toggleRepost>[0]) {
        try {
            await toggleRepost(post);
        } catch (e) {
            showError(isApiError(e) ? e.message : 'Failed to update repost.');
        }
    }

    async function handleAddComment(post: Parameters<typeof addComment>[0], content: string) {
        try {
            await addComment(post, content);
        } catch (e) {
            throw new Error(isApiError(e) ? e.message : 'Failed to add comment.');
        }
    }

    async function handleDeleteComment(post: Parameters<typeof removeComment>[0], commentId: string) {
        try {
            await removeComment(post, commentId);
        } catch (e) {
            showError(isApiError(e) ? e.message : 'Failed to delete comment.');
        }
    }

    return (
        <main className={styles.page}>
            <div className={styles.content}>
                {message && (
                    <p style={{ color: message.color, marginBottom: 16 }}>{message.text}</p>
                )}
                <PostFeed
                    posts={posts}
                    loading={loading}
                    error={error}
                    emptyText="No posts to show yet."
                    onToggleLike={handleToggleLike}
                    onToggleRepost={handleToggleRepost}
                    onAddComment={handleAddComment}
                    onDeleteComment={handleDeleteComment}
                />
            </div>
        </main>
    );
};

export default HomePage;
