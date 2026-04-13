import React from 'react';
import { useParams } from 'react-router-dom';
import { isApiError } from '../../../shared/api/base';
import { useToast } from '../../../shared/lib/hooks/useToast';
import { PostCard } from '../../../entities/post/ui/PostCard';
import { usePostDetails } from '../../../features/post/model/usePostDetails';
import styles from './PostDetailsPage.module.css';

const PostDetailsPage = () => {
    const { id } = useParams<{ id: string }>();
    const { message, showError, showSuccess } = useToast();
    const {
        post,
        loading,
        error,
        toggleLike,
        toggleRepost,
        addComment,
        removeComment,
    } = usePostDetails(id);

    async function handleToggleLike(targetPost: Parameters<typeof toggleLike>[0]) {
        try {
            await toggleLike(targetPost);
        } catch (e) {
            showError(isApiError(e) ? e.message : 'Failed to update like.');
        }
    }

    async function handleToggleRepost(targetPost: Parameters<typeof toggleRepost>[0]) {
        try {
            await toggleRepost(targetPost);
        } catch (e) {
            showError(isApiError(e) ? e.message : 'Failed to update repost.');
        }
    }

    async function handleAddComment(targetPost: Parameters<typeof addComment>[0], content: string) {
        try {
            await addComment(targetPost, content);
        } catch (e) {
            throw new Error(isApiError(e) ? e.message : 'Failed to add comment.');
        }
    }

    async function handleDeleteComment(targetPost: Parameters<typeof removeComment>[0], commentId: string) {
        try {
            await removeComment(targetPost, commentId);
            showSuccess('Comment deleted.');
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

                {loading && <div className={styles.message}>Loading post...</div>}
                {!loading && error && <div className={`${styles.message} ${styles.error}`}>{error}</div>}

                {!loading && !error && post && (
                    <PostCard
                        post={post}
                        onToggleLike={handleToggleLike}
                        onToggleRepost={handleToggleRepost}
                        onAddComment={handleAddComment}
                        onDeleteComment={handleDeleteComment}
                    />
                )}
            </div>
        </main>
    );
};

export default PostDetailsPage;
