import { useCallback, useEffect, useState } from 'react';
import { ApiError, isApiError } from '../../../shared/api/base';
import { deletePost, getUserPosts } from '../../../entities/post/api/posts.api';
import type { Post } from '../../../entities/post/model/types';

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

    return {
        posts,
        loading,
        error,
        refresh: loadPosts,
        removePost,
    };
}
