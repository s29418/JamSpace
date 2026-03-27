import { useCallback, useEffect, useState } from 'react';
import {
    getTeamInvites,
    getTeamInvitesByTeamId,
    postInviteUser,
    acceptTeamInvite,
    rejectTeamInvite,
    cancelTeamInvite,
} from 'entities/team/api/teamInvites.api';
import type { TeamInvite } from 'entities/team/model/types';
import { ApiError, isApiError } from 'shared/api/base';

type Options = {
    teamId?: string;
};

export function useTeamInvites(opts: Options = {}) {
    const { teamId } = opts;
    const [invites, setInvites] = useState<TeamInvite[]>([]);
    const [loading, setLoading] = useState<boolean>(true);
    const [error, setError] = useState<string | null>(null);

    const refresh = useCallback(async () => {
        setLoading(true);
        setError(null);
        try {
            const data = teamId ? await getTeamInvitesByTeamId(teamId) : await getTeamInvites();
            setInvites(data);
        } catch (e) {
            const msg = isApiError(e) ? (e as ApiError).message : 'Failed to load team-invites';
            setError(msg);
        } finally {
            setLoading(false);
        }
    }, [teamId]);

    useEffect(() => {
        void refresh();
    }, [refresh]);

    async function inviteUserToTeam(username: string, tid: string) {
        await postInviteUser(username, tid);
        await refresh();
    }
    async function acceptInvite(id: string) {
        await acceptTeamInvite(id);
        await refresh();
    }
    async function rejectInvite(id: string) {
        await rejectTeamInvite(id);
        await refresh();
    }
    async function cancelInvite(id: string) {
        await cancelTeamInvite(id);
        await refresh();
    }

    return {
        invites,
        loading,
        error,
        refresh,
        inviteUserToTeam,
        acceptInvite,
        rejectInvite,
        cancelInvite,
    };
}
