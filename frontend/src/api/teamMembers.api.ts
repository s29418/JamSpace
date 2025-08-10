import { api } from './api';

const API_URL = `/teams`;

export const changeTeamMemberRole = (
    teamId: string,
    userId: string,
    newRole: string
) => api.patch(`${API_URL}/${teamId}/members/${userId}/role?newRole=${encodeURIComponent(newRole)}`, {}, {
    headers: { 'Content-Type': 'application/json' },
});

export const kickTeamMember = (teamId: string, userId: string) =>
    api.delete(`${API_URL}/${teamId}/members/${userId}`);

export const changeTeamMemberMusicalRole = (
    teamId: string,
    userId: string,
    newMusicalRole: string
) => api.patch(`${API_URL}/${teamId}/members/${userId}/musical-role?musicalRole=${encodeURIComponent(newMusicalRole)}`, {}, {
    headers: { 'Content-Type': 'application/json' },
});

export const leaveTeam = (teamId: string) =>
    api.delete(`${API_URL}/${teamId}/members/leave`);
