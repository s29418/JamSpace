import { api } from './api';

export const postInviteUser = (username: string, teamId: string) =>
    api.post(`/teams/invites/${encodeURIComponent(username)}`, teamId, {
        headers: { 'Content-Type': 'application/json' },
    });