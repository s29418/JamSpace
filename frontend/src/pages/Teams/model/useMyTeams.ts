import { useCallback, useEffect, useState } from 'react';
import { getMyTeams } from 'entities/team/api/teams.api';
import type { Team } from 'entities/team/model/types';
import { ApiError, isApiError } from 'shared/lib/api/base';
import { onTeamRemoved, onTeamUpdated } from 'shared/lib/events/teamEvents';

export function useMyTeams() {
    const [teams, setTeams] = useState<Team[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    const refresh = useCallback(async () => {
        setLoading(true);
        setError(null);
        try {
            const data = await getMyTeams();
            setTeams(data);
        } catch (e) {
            const msg = isApiError(e) ? (e as ApiError).message : 'Failed to load teams';
            setError(msg);
        } finally {
            setLoading(false);
        }
    }, []);

    useEffect(() => {
        let alive = true;
        (async () => {
            try {
                setLoading(true);
                setError(null);
                const data = await getMyTeams();
                if (alive) setTeams(data);
            } catch (e) {
                if (alive) {
                    const msg = isApiError(e) ? (e as ApiError).message : 'Failed to load teams';
                    setError(msg);
                }
            } finally {
                if (alive) setLoading(false);
            }
        })();
        return () => { alive = false; };
    }, []);

    useEffect(() => {
        const offUpd = onTeamUpdated(({ teamId, patch }) => {
            if (patch && Object.keys(patch).length) {
                setTeams(prev => prev.map(t => (t.id === teamId ? { ...t, ...patch } : t)));
            } else {
                void refresh();
            }
        });

        const offRem = onTeamRemoved(({ teamId }) => {
            setTeams(prev => prev.filter(t => t.id !== teamId));
        });

        return () => { offUpd(); offRem(); };
    }, [refresh]);

    return { teams, setTeams, loading, error, refresh };
}
