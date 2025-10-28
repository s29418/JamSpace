import { useCallback, useEffect, useState } from 'react';
import {UserLite, UserProfile} from "../../../../entities/user/model/types";
import {getFollowers, getFollowing} from "../../../../entities/user/api/userFollows.api";
import {ApiError, isApiError} from "../../../../shared/lib/api/base";
import { getUserById } from "../../../../entities/user/api/profile.api";
import { useFollowActions } from './useFollowActions';

export type FollowsMode = 'followers' | 'following';

export function useUserFollows(userId?: string, mode: FollowsMode = 'followers') {
    const [items, setItems] = useState<UserLite[]>([]);
    const [loading, setLoading] = useState<boolean>(!!userId);
    const [error, setError] = useState<string | null>(null);
    const [ownerName, setOwnerName] = useState<string>('');

    const load = useCallback(async () => {
        if (!userId) return;
        setLoading(true);
        setError(null);
        try {
            const data =
                mode === 'followers'
                    ? await getFollowers(userId)
                    : await getFollowing(userId);
            setItems(data);
        } catch (e) {
            const msg = isApiError(e) ? (e as ApiError).message : 'Failed to load list';
            setError(msg);
        } finally {
            setLoading(false);
        }
    }, [userId, mode]);


    useEffect(() => {
        let alive = true;
        (async () => {
            if (!userId) return;
            try {
                const p: UserProfile = await getUserById(userId);
                if (alive) setOwnerName(p.displayName || p.username || '');
            } catch {
            }
        })();
        return () => {
            alive = false;
        };
    }, [userId]);

    const { toggleFollow, busy } = useFollowActions(
        () => items,
        (upd) => setItems(prev => upd(prev))
    );

    useEffect(() => {
        void load();
    }, [load]);

    return { items, loading, error, ownerName, toggleFollow, busy, refresh: load };
}
