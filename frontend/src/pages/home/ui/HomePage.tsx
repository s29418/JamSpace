import React from 'react';
import { usePostsFeed } from '../../../features/post/model/usePostsFeed';
import { PostFeed } from '../../../widgets/post-feed/ui/PostFeed';
import styles from './HomePage.module.css';
import { useToast } from '../../../shared/lib/hooks/useToast';
import { isApiError } from '../../../shared/api/base';
import { useAuthState } from '../../../shared/lib/hooks/useAuthState';
import { PostComposer } from '../../../features/post/ui/PostComposer';

const HomePage = () => {
    const { posts, loading, error, addPost, removePost, toggleLike, toggleRepost, addComment, removeComment } = usePostsFeed({ mode: 'auto' });
    const { isAuthenticated, currentUserId } = useAuthState();
    const { message, showError, showSuccess } = useToast();

    async function handleCreatePost(content: string, file?: File | null) {
        try {
            await addPost(content, file);
            showSuccess('Post published.');
        } catch (e) {
            throw new Error(isApiError(e) ? e.message : 'Failed to publish post.');
        }
    }

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

    async function handleDeletePost(postId: string) {
        try {
            await removePost(postId);
            showSuccess('Post deleted.');
        } catch (e) {
            showError(isApiError(e) ? e.message : 'Failed to delete post.');
        }
    }

    return (
        <main className={styles.page}>
            <div className={styles.content}>
                {message && (
                    <p style={{ color: message.color, marginBottom: 16, justifySelf: "center" }}>{message.text}</p>
                )}
                {isAuthenticated && <PostComposer onSubmit={handleCreatePost} />}
                <PostFeed
                    posts={posts}
                    loading={loading}
                    error={error}
                    emptyText="No posts to show yet."
                    currentUserId={currentUserId}
                    onDeletePost={handleDeletePost}
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
