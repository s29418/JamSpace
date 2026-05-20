import { api } from '../../../shared/api/base';
import type {
    CreateProjectNoteRequest,
    CreateTeamProjectRequest,
    EditProjectNoteRequest,
    EditTeamProjectRequest,
    ProjectAudioVersion,
    ProjectNote,
    TeamProject,
    UploadProjectAudioVersionRequest,
} from '../model/types';

const projectsRoot = (teamId: string) => `/teams/${teamId}/projects`;
const projectRoot = (teamId: string, projectId: string) => `${projectsRoot(teamId)}/${projectId}`;
const versionsRoot = (teamId: string, projectId: string) => `${projectRoot(teamId, projectId)}/versions`;
const notesRoot = (teamId: string, projectId: string) => `${projectRoot(teamId, projectId)}/notes`;

export const getTeamProjects = async (teamId: string) => {
    const res = await api.get<TeamProject[]>(projectsRoot(teamId));
    return res.data;
};

export const createTeamProject = async (teamId: string, payload: CreateTeamProjectRequest) => {
    const res = await api.post<TeamProject>(projectsRoot(teamId), payload);
    return res.data;
};

export const getTeamProjectById = async (teamId: string, projectId: string) => {
    const res = await api.get<TeamProject>(projectRoot(teamId, projectId));
    return res.data;
};

export const editTeamProject = async (
    teamId: string,
    projectId: string,
    payload: EditTeamProjectRequest
) => {
    const res = await api.put<TeamProject>(projectRoot(teamId, projectId), payload);
    return res.data;
};

export const deleteTeamProject = async (teamId: string, projectId: string) => {
    await api.delete(projectRoot(teamId, projectId));
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

export const getProjectAudioVersions = async (teamId: string, projectId: string) => {
    const res = await api.get<ProjectAudioVersion[]>(versionsRoot(teamId, projectId));
    return res.data;
};

export const uploadProjectAudioVersion = async (
    teamId: string,
    projectId: string,
    payload: UploadProjectAudioVersionRequest
) => {
    const formData = new FormData();
    formData.append('name', payload.name);
    if (payload.durationSeconds != null) {
        formData.append('durationSeconds', String(payload.durationSeconds));
    }
    formData.append('file', payload.file);

    const res = await api.post<ProjectAudioVersion>(versionsRoot(teamId, projectId), formData);
    return res.data;
};

export const deleteProjectAudioVersion = async (
    teamId: string,
    projectId: string,
    versionId: string
) => {
    await api.delete(`${versionsRoot(teamId, projectId)}/${versionId}`);
};

export const getProjectNotes = async (
    teamId: string,
    projectId: string,
    versionId?: string | null
) => {
    const res = await api.get<ProjectNote[]>(notesRoot(teamId, projectId), {
        params: versionId ? { versionId } : undefined,
    });
    return res.data;
};

export const createProjectNote = async (
    teamId: string,
    projectId: string,
    payload: CreateProjectNoteRequest
) => {
    const res = await api.post<ProjectNote>(notesRoot(teamId, projectId), payload);
    return res.data;
};

export const editProjectNote = async (
    teamId: string,
    projectId: string,
    noteId: string,
    payload: EditProjectNoteRequest
) => {
    const res = await api.put<ProjectNote>(`${notesRoot(teamId, projectId)}/${noteId}`, payload);
    return res.data;
};

export const completeProjectNote = async (teamId: string, projectId: string, noteId: string) => {
    const res = await api.patch<ProjectNote>(`${notesRoot(teamId, projectId)}/${noteId}/complete`);
    return res.data;
};

export const reopenProjectNote = async (teamId: string, projectId: string, noteId: string) => {
    const res = await api.patch<ProjectNote>(`${notesRoot(teamId, projectId)}/${noteId}/reopen`);
    return res.data;
};

export const deleteProjectNote = async (teamId: string, projectId: string, noteId: string) => {
    await api.delete(`${notesRoot(teamId, projectId)}/${noteId}`);
};
