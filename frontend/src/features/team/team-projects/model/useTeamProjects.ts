import { useCallback, useEffect, useState } from 'react';
import {
    createTeamProject,
    deleteTeamProject,
    editTeamProject,
    getTeamProjects,
    uploadTeamProjectPicture,
} from 'entities/team/api/teamProjects.api';
import type { CreateTeamProjectRequest, EditTeamProjectRequest, TeamProject } from 'entities/team/model/types';
import { ApiError, isApiError } from 'shared/api/base';

const byCreatedAtDesc = (a: TeamProject, b: TeamProject) =>
    new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime();

const getErrorMessage = (error: unknown, fallback: string) =>
    isApiError(error) ? (error as ApiError).message : fallback;

export function useTeamProjects(teamId?: string) {
    const [projects, setProjects] = useState<TeamProject[]>([]);
    const [loading, setLoading] = useState<boolean>(!!teamId);
    const [error, setError] = useState<string | null>(null);

    const refresh = useCallback(async () => {
        if (!teamId) return [];

        setLoading(true);
        setError(null);
        try {
            const data = await getTeamProjects(teamId);
            setProjects([...data].sort(byCreatedAtDesc));
            return data;
        } catch (e) {
            const message = getErrorMessage(e, 'Failed to load team projects');
            setError(message);
            throw e;
        } finally {
            setLoading(false);
        }
    }, [teamId]);

    useEffect(() => {
        let alive = true;
        if (!teamId) {
            setProjects([]);
            setLoading(false);
            return;
        }

        (async () => {
            setLoading(true);
            setError(null);
            try {
                const data = await getTeamProjects(teamId);
                if (alive) setProjects([...data].sort(byCreatedAtDesc));
            } catch (e) {
                if (alive) setError(getErrorMessage(e, 'Failed to load team projects'));
            } finally {
                if (alive) setLoading(false);
            }
        })();

        return () => { alive = false; };
    }, [teamId]);

    const createProject = useCallback(async (payload: CreateTeamProjectRequest) => {
        if (!teamId) throw new ApiError(400, 'Team is not selected');

        const created = await createTeamProject(teamId, payload);
        setProjects(prev => [created, ...prev].sort(byCreatedAtDesc));
        return created;
    }, [teamId]);

    const updateProjectPicture = useCallback(async (projectId: string, file: File) => {
        if (!teamId) throw new ApiError(400, 'Team is not selected');

        const pictureUrl = await uploadTeamProjectPicture(teamId, projectId, file);
        setProjects(prev => prev.map(project => (
            project.id === projectId ? { ...project, pictureUrl, updatedAt: new Date().toISOString() } : project
        )));
        return pictureUrl;
    }, [teamId]);

    const updateProject = useCallback(async (projectId: string, payload: EditTeamProjectRequest) => {
        if (!teamId) throw new ApiError(400, 'Team is not selected');

        const updated = await editTeamProject(teamId, projectId, payload);
        setProjects(prev => prev.map(project => (
            project.id === projectId ? updated : project
        )).sort(byCreatedAtDesc));
        return updated;
    }, [teamId]);

    const removeProject = useCallback(async (projectId: string) => {
        if (!teamId) throw new ApiError(400, 'Team is not selected');

        await deleteTeamProject(teamId, projectId);
        setProjects(prev => prev.filter(project => project.id !== projectId));
    }, [teamId]);

    return {
        projects,
        setProjects,
        loading,
        error,
        refresh,
        createProject,
        updateProjectPicture,
        updateProject,
        removeProject,
    };
}
