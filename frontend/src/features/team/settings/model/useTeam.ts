import { useCallback, useEffect, useState } from 'react';
import { getTeamById } from 'entities/team/api/teams.api';
import type { Team } from 'entities/team/model/types';
import { ApiError, isApiError } from 'shared/lib/api/base';
import { onTeamRemoved, onTeamUpdated } from 'shared/lib/events/teamEvents';

export function useTeam(teamId?: string) {
    const [team, setTeam] = useState<Team | null>(null);
    const [loading, setLoading] = useState<boolean>(!!teamId);
    const [error, setError] = useState<string | null>(null);

    const refresh = useCallback(async () => {
        if (!teamId) return;
        setLoading(true);
        setError(null);
        try {
            const data = await getTeamById(teamId);
            setTeam(data);
        } catch (e) {
            const msg = isApiError(e) ? (e as ApiError).message : 'Failed to load team';
            setError(msg);
        } finally {
            setLoading(false);
        }
    }, [teamId]);

    useEffect(() => {
        let alive = true;
        if (!teamId) return;
        (async () => {
            try {
                setLoading(true);
                setError(null);
                const data = await getTeamById(teamId);
                if (alive) setTeam(data);
            } catch (e) {
                if (alive) {
                    const msg = isApiError(e) ? (e as ApiError).message : 'Failed to load team';
                    setError(msg);
                }
            } finally {
                if (alive) setLoading(false);
            }
        })();
        return () => { alive = false; };
    }, [teamId]);

    useEffect(() => {
        const offUpd = onTeamUpdated(({ teamId: tid, patch }) => {
            if (tid !== teamId) return;
            if (patch && Object.keys(patch).length) {
                setTeam(prev => (prev ? { ...prev, ...patch } : prev));
            } else {
                void refresh();
            }
        });

        const offRem = onTeamRemoved(({ teamId: tid }) => {
            if (tid !== teamId) return;
            setTeam(null);
        });

        return () => { offUpd(); offRem(); };
    }, [teamId, refresh]);

    return { team, setTeam, loading, error, refresh };
}
