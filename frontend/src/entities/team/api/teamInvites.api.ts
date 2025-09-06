import { api } from '../../../shared/lib/api/base';
import type { TeamInvite } from '../model/types';

const ROOT = '/teams/invites';

export const getTeamInvites = async () => {
    const res = await api.get<TeamInvite[]>(`${ROOT}`);
    return res.data;
};

export const getTeamInvitesByTeamId = async (teamId: string) => {
    const res = await api.get<TeamInvite[]>(`${ROOT}/team/${teamId}`);
    return res.data;
};

export const postInviteUser = async (username: string, teamId: string) => {
    const res = await api.post<{ message?: string }>(`${ROOT}/${username}`, JSON.stringify(teamId), {
        headers: { 'Content-Type': 'application/json' },
    });
    return res.data;
};

export const acceptTeamInvite = async (inviteId: string) => {
    const res = await api.patch<{ message?: string }>(`${ROOT}/${inviteId}/accept`, {});
    return res.data;
};

export const rejectTeamInvite = async (inviteId: string) => {
    const res = await api.patch<{ message?: string }>(`${ROOT}/${inviteId}/reject`, {});
    return res.data;
};

export const cancelTeamInvite = async (inviteId: string) => {
    const res = await api.patch<{ message?: string }>(`${ROOT}/${inviteId}/cancel`, null);
    return res.data;
};
