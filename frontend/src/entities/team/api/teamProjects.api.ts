import { api } from '../../../shared/api/base';
import type { CreateTeamProjectRequest, TeamProject } from '../model/types';

const projectsRoot = (teamId: string) => `/teams/${teamId}/projects`;

export const getTeamProjects = async (teamId: string) => {
    const res = await api.get<TeamProject[]>(projectsRoot(teamId));
    return res.data;
};

export const createTeamProject = async (teamId: string, payload: CreateTeamProjectRequest) => {
    const res = await api.post<TeamProject>(projectsRoot(teamId), JSON.stringify(payload.name), {
        headers: { 'Content-Type': 'application/json' },
    });
    return res.data;
};

export const uploadTeamProjectPicture = async (teamId: string, projectId: string, file: File) => {
    const formData = new FormData();
    formData.append('file', file);

    const res = await api.patch<{ url: string }>(
        `${projectsRoot(teamId)}/${projectId}/picture`,
        formData,
        {
            headers: { 'Content-Type': 'multipart/form-data' },
        }
    );

    return res.data.url;
};
