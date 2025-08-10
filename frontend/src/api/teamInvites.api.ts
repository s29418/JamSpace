import { api } from './api';

const API_URL = '/teams/invites';

export const getTeamInvites = () =>
    api.get(`${API_URL}`);

export const getTeamInvitesByTeamId = (teamId: string) =>
    api.get(`${API_URL}/team/${teamId}`);

export const postInviteUser = (username: string, teamId: string) =>
    api.post(
        `${API_URL}/${username}`,
        JSON.stringify(teamId),
        {
            headers: { 'Content-Type': 'application/json' },
        }
    );

export const acceptTeamInvite = (inviteId: string) =>
    api.post(`${API_URL}/${inviteId}/accept`, {});

export const rejectTeamInvite = (inviteId: string) =>
    api.post(`${API_URL}/${inviteId}/reject`, {});

export const cancelTeamInvite = (inviteId: string) =>
    api.patch(`${API_URL}/${inviteId}/cancel`, null);