import { api } from './api';

const API_URL = '/teams';

export const getMyTeamsApi = () => api.get(`${API_URL}/my`);

export const uploadTeamPictureApi = (file: File) => {
    const formData = new FormData();
    formData.append('file', file);
    return api.post('/uploads/team-picture', formData, {
        headers: { 'Content-Type': 'multipart/form-data' },
    });
};

export const createTeamApi = (teamData: { name: string; teamPictureUrl?: string | null }) =>
    api.post(`${API_URL}`, teamData);

export const getTeamByIdApi = (id: string) => api.get(`${API_URL}/${id}`);

export const deleteTeamApi = (teamId: string) => api.delete(`${API_URL}/${teamId}`);

export const changeTeamNameApi = (teamId: string, newName: string) =>
    api.patch(`${API_URL}/${teamId}/teamName?teamName=${encodeURIComponent(newName)}`, {});

export const changeTeamPictureApi = (teamId: string, file: File) => {
    const formData = new FormData();
    formData.append('file', file);
    return api.patch(`${API_URL}/${teamId}/team-picture`, formData, {
        headers: { 'Content-Type': 'multipart/form-data' },
    });
};
