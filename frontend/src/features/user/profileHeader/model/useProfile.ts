import { useCallback, useEffect, useState } from 'react';
import {
    getUserById,
    updateUserProfile,
    UpdateUserProfileRequest, deleteUserAccount, changeUserPassword
} from '../../../../entities/user/api/profile.api';
import type {UserProfile, UserTag} from '../../../../entities/user/model/types';
import { ApiError, isApiError } from 'shared/lib/api/base';
import {addUserGenre, deleteUserGenre} from "../../../../entities/user/api/genres.api";
import {addUserSkill, deleteUserSkill} from "../../../../entities/user/api/skills.api";
import {useFollowActions} from "./useFollowActions";

export function useProfile(userId?: string) {
    const [profile, setProfile] = useState<UserProfile | null>(null);
    const [loading, setLoading] = useState<boolean>(!!userId);
    const [error, setError] = useState<string | null>(null);
    const [followLoading] = useState(false);

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

    const { toggleFollow: toggleFollowShared } = useFollowActions(
        () => (profile ? [profile] : []),
        (upd) => {
            const next = upd(profile ? [profile] : []);
            setProfile(next[0] ?? null);
        }
    );

    const toggleFollow = useCallback(async () => {
        if (!profile) return;
        await toggleFollowShared(profile.id);
    }, [profile, toggleFollowShared]);


    const updateProfile = useCallback(async (draft: UpdateUserProfileRequest, file?: File) => {
        if (!userId) return;
        try {
            await updateUserProfile(userId, draft, file);
        } catch (e) {
            throw isApiError(e) ? e : new ApiError(0, 'Failed to update profile');
        }

        if (file) {
            await refresh();
            return;
        }
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
    }, [userId, setProfile, refresh]);

    const addGenre = useCallback(async (name: string) => {
        if (!profile) throw new ApiError(400, 'Profile not loaded');
        try {
            const tag = await addUserGenre(profile.id, name);
            setProfile(p => p ? { ...p, genres: [...(p.genres ?? []), tag] } : p);
        } catch (e) {
            throw isApiError(e) ? e : new ApiError(0, 'Failed to add genre');
        }
    }, [profile, setProfile]);

    const removeGenre = useCallback(async (id: string) => {
        if (!profile) throw new ApiError(400, 'Profile not loaded');
        try {
            await deleteUserGenre(profile.id, id);
            setProfile(p => p ? { ...p, genres: (p.genres ?? []).filter(g => g.id !== id) } : p);
        } catch (e) {
            throw isApiError(e) ? e : new ApiError(0, 'Failed to remove genre');
        }
    }, [profile, setProfile]);

    const addSkill = useCallback(async (name: string) => {
        if (!profile) throw new ApiError(400, 'Profile not loaded');
        try {
            const tag = await addUserSkill(profile.id, name);
            setProfile(p => p ? { ...p, skills: [...(p.skills ?? []), tag] } : p);
        } catch (e) {
            throw isApiError(e) ? e : new ApiError(0, 'Failed to add genre');
        }
    }, [profile, setProfile]);

    const removeSkill = useCallback(async (skillId: string) => {
        if (!profile) throw new ApiError(400, 'Profile not loaded');
        try {
            await deleteUserSkill(profile.id, skillId);
            setProfile(p => p ? { ...p, skills: (p.skills ?? []).filter(g => g.id !== skillId) } : p);
        } catch (e) {
            throw isApiError(e) ? e : new ApiError(0, 'Failed to remove genre');
        }
    }, [profile, setProfile]);

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
        window.location.href = '/';
    }, [userId]);


    return { profile, setProfile, loading, error, refresh, toggleFollow, followLoading, updateProfile
        , addGenre, removeGenre, addSkill, removeSkill, updateEmail, changePassword, deleteAccount };
}