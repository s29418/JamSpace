import { useCallback, useEffect, useState } from 'react';
import { ApiError, isApiError } from '../../../shared/api/base';
import { getCurrentUserId, getToken } from '../../../shared/lib/auth/token';
import {
    createPost,
    createComment,
    deleteComment,
    deletePost,
    deleteRepost,
    getExploreFeed,
    getFollowedFeed,
    likePost,
    repostPost,
    unlikePost,
} from '../../../entities/post/api/posts.api';
import type { Post } from '../../../entities/post/model/types';
import { removeOwnRepostsForOriginal, updatePostsById } from './postState';

type Options = {
    mode?: 'auto' | 'feed' | 'explore';
};

export function usePostsFeed(options: Options = {}) {
    const mode = options.mode ?? 'auto';
    const [posts, setPosts] = useState<Post[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    const loadPosts = useCallback(async () => {
        setLoading(true);
        setError(null);

        try {
            const resolvedMode =
                mode === 'auto'
                    ? (getToken() ? 'feed' : 'explore')
                    : mode;

            const result = resolvedMode === 'feed'
                ? await getFollowedFeed()
                : await getExploreFeed();

            setPosts(result.items);
        } catch (e) {
            const message = isApiError(e) ? (e as ApiError).message : 'Failed to load posts';
            setError(message);
        } finally {
            setLoading(false);
        }
    }, [mode]);

    useEffect(() => {
        void loadPosts();
    }, [loadPosts]);

    const removePost = useCallback(async (postId: string) => {
        await deletePost(postId);
        setPosts((current) => current.filter((post) => post.id !== postId));
    }, []);

    const addPost = useCallback(async (content: string, file?: File | null) => {
        const createdPost = await createPost(content, file);
        setPosts((current) => [createdPost, ...current]);
        return createdPost;
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

        setPosts((current) => {
            const updated = updatePostsById(current, post.id, (target) => ({
                ...target,
                isRepostedByCurrentUser: true,
                repostCount: target.repostCount + 1,
            }));

            if (updated.some((item) => item.id === createdRepost.id)) {
                return updated;
            }

            return [createdRepost, ...updated];
        });
    }, []);

    const addComment = useCallback(async (post: Post, content: string) => {
        const createdComment = await createComment(post.id, content);

        setPosts((current) =>
            updatePostsById(current, post.id, (target) => ({
                ...target,
                commentCount: target.commentCount + 1,
                comments: [createdComment, ...target.comments],
            })),
        );

        return createdComment;
    }, []);

    const removeComment = useCallback(async (post: Post, commentId: string) => {
        await deleteComment(post.id, commentId);

        setPosts((current) =>
            updatePostsById(current, post.id, (target) => ({
                ...target,
                commentCount: Math.max(0, target.commentCount - 1),
                comments: target.comments.filter((comment) => comment.id !== commentId),
            })),
        );
    }, []);

    return {
        posts,
        loading,
        error,
        refresh: loadPosts,
        addPost,
        removePost,
        toggleLike,
        toggleRepost,
        addComment,
        removeComment,
    };
}
