import { useCallback, useEffect, useState } from 'react';
import { ApiError, isApiError } from '../../../shared/api/base';
import {
    createComment,
    deleteComment,
    deleteRepost,
    getPost,
    likePost,
    repostPost,
    unlikePost,
} from '../../../entities/post/api/posts.api';
import type { Post } from '../../../entities/post/model/types';

export function usePostDetails(postId?: string) {
    const [post, setPost] = useState<Post | null>(null);
    const [loading, setLoading] = useState(Boolean(postId));
    const [error, setError] = useState<string | null>(null);

    const loadPost = useCallback(async () => {
        if (!postId) {
            setPost(null);
            setLoading(false);
            setError('Post not found.');
            return;
        }

        setLoading(true);
        setError(null);

        try {
            const result = await getPost(postId);
            setPost(result);
        } catch (e) {
            const message = isApiError(e) ? (e as ApiError).message : 'Failed to load post';
            setError(message);
        } finally {
            setLoading(false);
        }
    }, [postId]);

    useEffect(() => {
        void loadPost();
    }, [loadPost]);

    const toggleLike = useCallback(async (targetPost: Post) => {
        if (targetPost.isLikedByCurrentUser) {
            await unlikePost(targetPost.id);
        } else {
            await likePost(targetPost.id);
        }

        setPost((current) => current ? {
            ...current,
            isLikedByCurrentUser: !current.isLikedByCurrentUser,
            likeCount: Math.max(0, current.likeCount + (current.isLikedByCurrentUser ? -1 : 1)),
        } : current);
    }, []);

    const toggleRepost = useCallback(async (targetPost: Post) => {
        if (targetPost.isRepostedByCurrentUser) {
            await deleteRepost(targetPost.id);

            setPost((current) => current ? {
                ...current,
                isRepostedByCurrentUser: false,
                repostCount: Math.max(0, current.repostCount - 1),
            } : current);

            return;
        }

        await repostPost(targetPost.id);

        setPost((current) => current ? {
            ...current,
            isRepostedByCurrentUser: true,
            repostCount: current.repostCount + 1,
        } : current);
    }, []);

    const addComment = useCallback(async (targetPost: Post, content: string) => {
        const createdComment = await createComment(targetPost.id, content);

        setPost((current) => current ? {
            ...current,
            commentCount: current.commentCount + 1,
            comments: [createdComment, ...current.comments],
        } : current);

        return createdComment;
    }, []);

    const removeComment = useCallback(async (targetPost: Post, commentId: string) => {
        await deleteComment(targetPost.id, commentId);

        setPost((current) => current ? {
            ...current,
            commentCount: Math.max(0, current.commentCount - 1),
            comments: current.comments.filter((comment) => comment.id !== commentId),
        } : current);
    }, []);

    return {
        post,
        loading,
        error,
        refresh: loadPost,
        toggleLike,
        toggleRepost,
        addComment,
        removeComment,
    };
}
