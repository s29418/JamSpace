import { useCallback, useEffect, useState } from 'react';
import { ApiError, isApiError } from '../../../shared/api/base';
import { getCurrentUserId } from '../../../shared/lib/auth/token';
import {
    deletePost,
    deleteRepost,
    getUserPosts,
    likePost,
    repostPost,
    unlikePost,
} from '../../../entities/post/api/posts.api';
import type { Post } from '../../../entities/post/model/types';
import { removeOwnRepostsForOriginal, updatePostsById } from './postState';

export function useUserPosts(userId?: string) {
    const [posts, setPosts] = useState<Post[]>([]);
    const [loading, setLoading] = useState(Boolean(userId));
    const [error, setError] = useState<string | null>(null);

    const loadPosts = useCallback(async () => {
        if (!userId) {
            setPosts([]);
            setLoading(false);
            return;
        }

        setLoading(true);
        setError(null);

        try {
            const result = await getUserPosts(userId);
            setPosts(result.items);
        } catch (e) {
            const message = isApiError(e) ? (e as ApiError).message : 'Failed to load posts';
            setError(message);
        } finally {
            setLoading(false);
        }
    }, [userId]);

    useEffect(() => {
        void loadPosts();
    }, [loadPosts]);

    const removePost = useCallback(async (postId: string) => {
        await deletePost(postId);
        setPosts((current) => current.filter((post) => post.id !== postId));
    }, []);

    const toggleLike = useCallback(async (post: Post) => {
        if (post.isLikedByCurrentUser) {
            await unlikePost(post.id);
        } else {
            await likePost(post.id);
        }

        setPosts((current) =>
            updatePostsById(current, post.id, (target) => ({
                ...target,
                isLikedByCurrentUser: !target.isLikedByCurrentUser,
                likeCount: Math.max(0, target.likeCount + (target.isLikedByCurrentUser ? -1 : 1)),
            })),
        );
    }, []);

    const toggleRepost = useCallback(async (post: Post) => {
        if (post.isRepostedByCurrentUser) {
            await deleteRepost(post.id);

            setPosts((current) => {
                const updated = updatePostsById(current, post.id, (target) => ({
                    ...target,
                    isRepostedByCurrentUser: false,
                    repostCount: Math.max(0, target.repostCount - 1),
                }));

                return removeOwnRepostsForOriginal(updated, post.id, getCurrentUserId());
            });

            return;
        }

        const createdRepost = await repostPost(post.id);
        const currentUserId = getCurrentUserId();

        setPosts((current) => {
            const updated = updatePostsById(current, post.id, (target) => ({
                ...target,
                isRepostedByCurrentUser: true,
                repostCount: target.repostCount + 1,
            }));

            if (!currentUserId || currentUserId !== userId || updated.some((item) => item.id === createdRepost.id)) {
                return updated;
            }

            return [createdRepost, ...updated];
        });
    }, [userId]);

    return {
        posts,
        loading,
        error,
        refresh: loadPosts,
        removePost,
        toggleLike,
        toggleRepost,
    };
}
