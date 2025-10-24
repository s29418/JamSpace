import { useCallback, useEffect, useState } from 'react';
import {
    getUserById,
    followUser,
    unfollowUser,
    updateUserProfile,
    UpdateUserProfileRequest, deleteUserAccount, changeUserPassword
} from '../../../../entities/user/api/profile.api';
import type {UserProfile, UserTag} from '../../../../entities/user/model/types';
import { ApiError, isApiError } from 'shared/lib/api/base';
import {addUserGenre, deleteUserGenre} from "../../../../entities/user/api/genres.api";
import {addUserSkill, deleteUserSkill} from "../../../../entities/user/api/skills.api";

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

    const addGenre = useCallback(async (name: string) => {
        if (!userId || !name.trim()) return;
        const optimistic: UserTag = { id: `tmp-${Math.random().toString(36).slice(2)}`, name: name.trim() };
        setProfile(prev => prev ? { ...prev, genres: [...(prev.genres ?? []), optimistic] } : prev);
        try {
            const created = await addUserGenre(userId, name.trim());
            setProfile(prev => prev ? {
                ...prev,
                genres: (prev.genres ?? []).map(g => g.id === optimistic.id ? created : g)
            } : prev);
        } catch (e) {
            setProfile(prev => prev ? { ...prev, genres: (prev.genres ?? []).filter(g => g.id !== optimistic.id) } : prev);
            throw e;
        }
    }, [userId, setProfile]);

    const removeGenre = useCallback(async (genreId: string) => {
        if (!userId) return;
        const prevSnapshot = profile?.genres ?? [];
        setProfile(prev => prev ? { ...prev, genres: (prev.genres ?? []).filter(g => g.id !== genreId) } : prev);
        try {
            await deleteUserGenre(userId, genreId);
        } catch (e) {
            setProfile(prev => prev ? { ...prev, genres: prevSnapshot } : prev);
            throw e;
        }
    }, [userId, profile, setProfile]);

    const addSkill = useCallback(async (name: string) => {
        if (!userId || !name.trim()) return;
        const optimistic: UserTag = { id: `tmp-${Math.random().toString(36).slice(2)}`, name: name.trim() };
        setProfile(prev => prev ? { ...prev, skills: [...(prev.skills ?? []), optimistic] } : prev);
        try {
            const created = await addUserSkill(userId, name.trim());
            setProfile(prev => prev ? {
                ...prev,
                skills: (prev.skills ?? []).map(s => s.id === optimistic.id ? created : s)
            } : prev);
        } catch (e) {
            setProfile(prev => prev ? { ...prev, skills: (prev.skills ?? []).filter(s => s.id !== optimistic.id) } : prev);
            throw e;
        }
    }, [userId, setProfile]);

    const removeSkill = useCallback(async (skillId: string) => {
        if (!userId) return;
        const prevSnapshot = profile?.skills ?? [];
        setProfile(prev => prev ? { ...prev, skills: (prev.skills ?? []).filter(s => s.id !== skillId) } : prev);
        try {
            await deleteUserSkill(userId, skillId);
        } catch (e) {
            setProfile(prev => prev ? { ...prev, skills: prevSnapshot } : prev);
            throw e;
        }
    }, [userId, profile, setProfile]);

    const updateEmail = useCallback(async (email: string) => {
        if (!userId) return;
        await updateUserProfile(userId, { setEmail: true, email });
        setProfile(prev => (prev ? { ...prev, email } : prev));
    }, [userId, setProfile]);

    const changePassword = useCallback(async (currentPassword: string, newPassword: string) => {
        if (!userId) return;
        await changeUserPassword(userId, { currentPassword, newPassword });
    }, [userId]);

    const deleteAccount = useCallback(async () => {
        if (!userId) return;
        await deleteUserAccount(userId);
        localStorage.removeItem('token');
        window.location.href = '/'; // lub reload()
    }, [userId]);


    return { profile, setProfile, loading, error, refresh, toggleFollow, followLoading, updateProfile
        , addGenre, removeGenre, addSkill, removeSkill, updateEmail, changePassword, deleteAccount };
}