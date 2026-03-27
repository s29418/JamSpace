import { useCallback, useState } from "react";
import { followUser, unfollowUser } from "entities/user/api/userFollows.api";

type Item = {
    id: string;
    isFollowing: boolean;
    isMe: boolean;
    followersCount: number;
};

export function useSearchFollowActions<T extends Item>(
    getItems: () => T[],
    setItems: (update: (prev: T[]) => T[]) => void
) {
    const [busy, setBusy] = useState<Record<string, boolean>>({});

    const toggleFollow = useCallback(
        async (targetId: string) => {
            if (!targetId || busy[targetId]) return;

            const current = getItems().find((u) => u.id === targetId);
            if (!current || current.isMe) return;

            setBusy((b) => ({ ...b, [targetId]: true }));

            setItems((prev) =>
                prev.map((u) => {
                    if (u.id !== targetId) return u;

                    const nextFollowing = !u.isFollowing;
                    const nextFollowers = nextFollowing ? u.followersCount + 1 : Math.max(0, u.followersCount - 1);

                    return { ...u, isFollowing: nextFollowing, followersCount: nextFollowers };
                })
            );

            try {
                if (current.isFollowing) {
                    await unfollowUser(targetId);
                } else {
                    await followUser(targetId);
                }
            } catch {
                setItems((prev) =>
                    prev.map((u) => {
                        if (u.id !== targetId) return u;

                        const rollbackFollowing = !u.isFollowing;
                        const rollbackFollowers = rollbackFollowing ? u.followersCount + 1 : Math.max(0, u.followersCount - 1);

                        return { ...u, isFollowing: rollbackFollowing, followersCount: rollbackFollowers };
                    })
                );
            } finally {
                setBusy((b) => ({ ...b, [targetId]: false }));
            }
        },
        [busy, getItems, setItems]
    );

    return { toggleFollow, busy };
}
