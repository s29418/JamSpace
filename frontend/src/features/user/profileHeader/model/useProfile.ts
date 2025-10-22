import { useCallback, useEffect, useState } from 'react';
import { getUserById } from '../../../../entities/user/api/profile.api';
import type { UserProfile } from '../../../../entities/user/model/types';
import { ApiError, isApiError } from '../../../../shared/lib/api/base';


export function useProfile(userId?: string) {
    const [profile, setProfile] = useState<UserProfile | null>(null);
    const [loading, setLoading] = useState<boolean>(!!userId);
    const [error, setError] = useState<string | null>(null);

    const refresh = useCallback(async () => {
        if (!userId) return;
        setLoading(true);
        setError(null);
        try {
            const data = await getUserById(userId);
            setProfile(data);
        } catch (e) {
            const msg = isApiError(e) ? (e as ApiError).message : 'Failed to load profile';
            setError(msg);
        } finally {
            setLoading(false);
        }
    }, [userId]);

    useEffect(() => {
        let alive = true;
        if (!userId) return;
        (async () => {
            try {
                setLoading(true);
                setError(null);
                const data = await getUserById(userId);
                if (alive) setProfile(data);
            } catch (e) {
                if (alive) {
                    const msg = isApiError(e) ? (e as ApiError).message : 'Failed to load profile';
                    setError(msg);
                }
            } finally {
                if (alive) setLoading(false);
            }
        })();
        return () => { alive = false; };
    }, [userId]);

    return { profile, setProfile, loading, error, refresh };
}