import { useCallback, useEffect, useState } from 'react';
import { ApiError, isApiError } from '../../../shared/api/base';
import { getToken } from '../../../shared/lib/auth/token';
import { deletePost, getExploreFeed, getFollowedFeed } from '../../../entities/post/api/posts.api';
import type { Post } from '../../../entities/post/model/types';

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

    return {
        posts,
        loading,
        error,
        refresh: loadPosts,
        removePost,
    };
}
