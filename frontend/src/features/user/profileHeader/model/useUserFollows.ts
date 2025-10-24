import { useCallback, useEffect, useState } from 'react';
import {UserLite} from "../../../../entities/user/model/types";
import {getFollowers, getFollowing} from "../../../../entities/user/api/userFollows.api";
import {ApiError, isApiError} from "../../../../shared/lib/api/base";


export type FollowsMode = 'followers' | 'following';

export function useUserFollows(userId?: string, mode: FollowsMode = 'followers') {
    const [items, setItems] = useState<UserLite[]>([]);
    const [loading, setLoading] = useState<boolean>(!!userId);
    const [error, setError] = useState<string | null>(null);

    const load = useCallback(async () => {
        if (!userId) return;
        setLoading(true);
        setError(null);
        try {
            const data = mode === 'followers' ? await getFollowers(userId) : await getFollowing(userId);
            setItems(data);
        } catch (e) {
            const msg = isApiError(e) ? (e as ApiError).message : 'Failed to load list';
            setError(msg);
        } finally {
            setLoading(false);
        }
    }, [userId, mode]);

    useEffect(() => { void load(); }, [load]);

    return { items, loading, error, refresh: load };
}