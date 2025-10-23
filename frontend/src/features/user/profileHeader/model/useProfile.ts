import { useCallback, useEffect, useState } from 'react';
import {
    getUserById,
    followUser,
    unfollowUser,
    updateUserProfile,
    UpdateUserProfileRequest
} from '../../../../entities/user/api/profile.api';
import type { UserProfile } from '../../../../entities/user/model/types';
import { ApiError, isApiError } from 'shared/lib/api/base';

export function useProfile(userId?: string) {
    const [profile, setProfile] = useState<UserProfile | null>(null);
    const [loading, setLoading] = useState<boolean>(!!userId);
    const [error, setError] = useState<string | null>(null);

    const [followLoading, setFollowLoading] = useState(false);

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

    const toggleFollow = useCallback(async () => {
        if (!profile?.id || followLoading) return;
        setFollowLoading(true);

        const prev = profile;
        const next: UserProfile = {
            ...prev,
            isFollowing: !prev.isFollowing,
            followersCount: (prev.followersCount ?? 0) + (prev.isFollowing ? -1 : 1),
        };

        setProfile(next);

        try {
            if (prev.isFollowing) {
                await unfollowUser(prev.id);
            } else {
                await followUser(prev.id);
            }
        } catch (e) {
            setProfile(prev);
            const msg = isApiError(e) ? (e as ApiError).message : 'Follow action failed';
            setError(msg);
        } finally {
            setFollowLoading(false);
        }
    }, [profile, followLoading]);


    const updateProfile = useCallback(async (draft: UpdateUserProfileRequest, file?: File) => {
        if (!userId) return;

        await updateUserProfile(userId, draft, file);

        if (file) {
            await refresh();
        } else {
            setProfile(prev => {
                if (!prev) return prev;
                const patch: Partial<UserProfile> = {};
                if (draft.setDisplayName)    patch.displayName = draft.displayName ?? '';
                if (draft.setBio)            patch.bio = draft.bio ?? '';
                if (draft.setLocation)       patch.location = {
                    city: draft.location?.city ?? '',
                    country: draft.location?.country ?? ''
                };
                if (draft.setProfilePicture) patch.profilePictureUrl = draft.profilePictureUrl ?? '';
                return { ...prev, ...patch };
            });
        }
    }, [userId, setProfile, refresh]);


    return { profile, setProfile, loading, error, refresh, toggleFollow, followLoading, updateProfile };
}

