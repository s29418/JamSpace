import { useState, useCallback } from 'react';
import {followUser, unfollowUser} from "../../../../entities/user/api/userFollows.api";


export function useFollowActions<T extends { id: string; isFollowing?: boolean }>(
    getItems: () => T[],
    setItems: (update: (prev: T[]) => T[]) => void
) {
    const [busy, setBusy] = useState<Record<string, boolean>>({});

    const toggleFollow = useCallback(async (targetId: string) => {
        if (!targetId || busy[targetId]) return;
        setBusy(b => ({ ...b, [targetId]: true }));

        setItems(prev => prev.map(u => u.id === targetId ? { ...u, isFollowing: !u.isFollowing } : u));

        try {
            const curr = getItems().find(u => u.id === targetId);
            if (curr?.isFollowing) {
                await unfollowUser(targetId);
            } else {
                await followUser(targetId);
            }
        } catch {
            setItems(prev => prev.map(u => u.id === targetId ? { ...u, isFollowing: !u.isFollowing } : u));
        } finally {
            setBusy(b => ({ ...b, [targetId]: false }));
        }
    }, [busy, getItems, setItems]);

    return { toggleFollow, busy };
}
