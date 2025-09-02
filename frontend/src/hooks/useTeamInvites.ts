import { useCallback, useEffect, useState } from 'react';
import type { TeamInvite } from '../types/team';
import { fetchTeamInvitesByTeamId } from '../services/teamInvites.service';

export function useTeamInvites(teamId: string) {
    const [invites, setInvites] = useState<TeamInvite[]>([]);
    const [loading, setLoading] = useState<boolean>(true);
    const [error, setError] = useState<string | null>(null);

    const refreshInvites = useCallback(async () => {
        setLoading(true);
        setError(null);
        try {
            const data = await fetchTeamInvitesByTeamId(teamId);
            setInvites(data);
        } catch (e: any) {
            setError(e?.message ?? 'Failed to load invites.');
        } finally {
            setLoading(false);
        }
    }, [teamId]);

    useEffect(() => {
        if (!teamId) return;
        refreshInvites();
    }, [teamId, refreshInvites]);

    return { invites, setInvites, loading, error, refreshInvites };
}
