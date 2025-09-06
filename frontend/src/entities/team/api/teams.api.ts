import { api } from '../../../shared/lib/api/base';
import type { Team } from '../model/types';

const ROOT = '/teams';

export const getMyTeams = async () => {
    const res = await api.get<Team[]>(`${ROOT}/my`);
    return res.data;
};

export const createTeam = async (teamData: { name: string; teamPictureUrl?: string | null }) => {
    const res = await api.post<Team>(`${ROOT}`, teamData);
    return res.data;
};

export const getTeamById = async (id: string) => {
    const res = await api.get<Team>(`${ROOT}/${id}`);
    return res.data;
};

export const deleteTeam = async (teamId: string) => {
    const res = await api.delete<{ message?: string }>(`${ROOT}/${teamId}`);
    return res.data;
};

export const changeTeamName = async (teamId: string, newName: string) => {
    const res = await api.patch<{ message?: string }>(
        `${ROOT}/${teamId}/teamName?teamName=${encodeURIComponent(newName)}`,
        {}
    );
    return res.data;
};

export const changeTeamPicture = async (teamId: string, file: File) => {
    const formData = new FormData();
    formData.append('file', file);
    const res = await api.patch<{ message?: string }>(`${ROOT}/${teamId}/team-picture`, formData, {
        headers: { 'Content-Type': 'multipart/form-data' },
    });
    return res.data;
};
