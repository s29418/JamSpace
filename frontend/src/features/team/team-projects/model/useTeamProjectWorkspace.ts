import { useCallback, useEffect, useMemo, useState } from 'react';
import {
    completeProjectNote,
    createProjectNote,
    deleteProjectAudioVersion,
    deleteProjectNote,
    deleteTeamProject,
    editProjectNote,
    editTeamProject,
    getProjectAudioVersions,
    getProjectNotes,
    getTeamProjectById,
    reopenProjectNote,
    uploadProjectAudioVersion,
    uploadTeamProjectPicture,
} from 'entities/team/api/teamProjects.api';
import type {
    CreateProjectNoteRequest,
    EditTeamProjectRequest,
    EditProjectNoteRequest,
    ProjectAudioVersion,
    ProjectNote,
    TeamProject,
    UploadProjectAudioVersionRequest,
} from 'entities/team/model/types';
import { ApiError, isApiError } from 'shared/api/base';

const byCreatedAtDesc = <T extends { createdAt: string }>(a: T, b: T) =>
    new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime();

const byNoteOrder = (a: ProjectNote, b: ProjectNote) => {
    if (a.status !== b.status) return a.status === 'Active' ? -1 : 1;
    return byCreatedAtDesc(a, b);
};

const getErrorMessage = (error: unknown, fallback: string) =>
    isApiError(error) ? (error as ApiError).message : fallback;

export function useTeamProjectWorkspace(teamId?: string, projectId?: string) {
    const [project, setProject] = useState<TeamProject | null>(null);
    const [versions, setVersions] = useState<ProjectAudioVersion[]>([]);
    const [notes, setNotes] = useState<ProjectNote[]>([]);
    const [selectedVersionId, setSelectedVersionId] = useState<string | null>(null);
    const [loading, setLoading] = useState<boolean>(!!teamId && !!projectId);
    const [error, setError] = useState<string | null>(null);

    const selectedVersion = useMemo(
        () => versions.find(version => version.id === selectedVersionId) ?? null,
        [selectedVersionId, versions]
    );

    const refreshNotes = useCallback(async (versionId?: string | null) => {
        if (!teamId || !projectId) return [];

        const data = await getProjectNotes(teamId, projectId, versionId);
        setNotes([...data].sort(byNoteOrder));
        return data;
    }, [projectId, teamId]);

    const refresh = useCallback(async () => {
        if (!teamId || !projectId) return null;

        setLoading(true);
        setError(null);
        try {
            const [projectData, versionsData, notesData] = await Promise.all([
                getTeamProjectById(teamId, projectId),
                getProjectAudioVersions(teamId, projectId),
                getProjectNotes(teamId, projectId),
            ]);

            const sortedVersions = [...versionsData].sort(byCreatedAtDesc);
            setProject(projectData);
            setVersions(sortedVersions);
            setNotes([...notesData].sort(byNoteOrder));
            setSelectedVersionId(current => {
                if (current && sortedVersions.some(version => version.id === current)) return current;
                return sortedVersions[0]?.id ?? null;
            });

            return projectData;
        } catch (e) {
            const message = getErrorMessage(e, 'Failed to load project');
            setError(message);
            throw e;
        } finally {
            setLoading(false);
        }
    }, [projectId, teamId]);

    useEffect(() => {
        let alive = true;

        if (!teamId || !projectId) {
            setProject(null);
            setVersions([]);
            setNotes([]);
            setSelectedVersionId(null);
            setLoading(false);
            return;
        }

        (async () => {
            setLoading(true);
            setError(null);
            try {
                const [projectData, versionsData, notesData] = await Promise.all([
                    getTeamProjectById(teamId, projectId),
                    getProjectAudioVersions(teamId, projectId),
                    getProjectNotes(teamId, projectId),
                ]);

                if (!alive) return;

                const sortedVersions = [...versionsData].sort(byCreatedAtDesc);
                setProject(projectData);
                setVersions(sortedVersions);
                setNotes([...notesData].sort(byNoteOrder));
                setSelectedVersionId(sortedVersions[0]?.id ?? null);
            } catch (e) {
                if (alive) setError(getErrorMessage(e, 'Failed to load project'));
            } finally {
                if (alive) setLoading(false);
            }
        })();

        return () => { alive = false; };
    }, [projectId, teamId]);

    const uploadVersion = useCallback(async (payload: UploadProjectAudioVersionRequest) => {
        if (!teamId || !projectId) throw new ApiError(400, 'Project is not selected');

        const created = await uploadProjectAudioVersion(teamId, projectId, payload);
        setVersions(prev => [created, ...prev].sort(byCreatedAtDesc));
        setSelectedVersionId(current => current ?? created.id);
        return created;
    }, [projectId, teamId]);

    const removeVersion = useCallback(async (versionId: string) => {
        if (!teamId || !projectId) throw new ApiError(400, 'Project is not selected');

        await deleteProjectAudioVersion(teamId, projectId, versionId);
        setVersions(prev => {
            const next = prev.filter(version => version.id !== versionId);
            setSelectedVersionId(current => current === versionId ? next[0]?.id ?? null : current);
            return next;
        });
        await refreshNotes();
    }, [projectId, refreshNotes, teamId]);

    const updateProject = useCallback(async (payload: EditTeamProjectRequest) => {
        if (!teamId || !projectId) throw new ApiError(400, 'Project is not selected');

        const updated = await editTeamProject(teamId, projectId, payload);
        setProject(updated);
        return updated;
    }, [projectId, teamId]);

    const updateProjectPicture = useCallback(async (file: File) => {
        if (!teamId || !projectId) throw new ApiError(400, 'Project is not selected');

        const pictureUrl = await uploadTeamProjectPicture(teamId, projectId, file);
        setProject(prev => prev ? { ...prev, pictureUrl, updatedAt: new Date().toISOString() } : prev);
        return pictureUrl;
    }, [projectId, teamId]);

    const removeProject = useCallback(async () => {
        if (!teamId || !projectId) throw new ApiError(400, 'Project is not selected');

        await deleteTeamProject(teamId, projectId);
    }, [projectId, teamId]);

    const addNote = useCallback(async (payload: CreateProjectNoteRequest) => {
        if (!teamId || !projectId) throw new ApiError(400, 'Project is not selected');

        const created = await createProjectNote(teamId, projectId, payload);
        setNotes(prev => [created, ...prev].sort(byNoteOrder));
        return created;
    }, [projectId, teamId]);

    const updateNote = useCallback(async (noteId: string, payload: EditProjectNoteRequest) => {
        if (!teamId || !projectId) throw new ApiError(400, 'Project is not selected');

        const updated = await editProjectNote(teamId, projectId, noteId, payload);
        setNotes(prev => prev.map(note => note.id === noteId ? updated : note).sort(byNoteOrder));
        return updated;
    }, [projectId, teamId]);

    const completeNote = useCallback(async (noteId: string) => {
        if (!teamId || !projectId) throw new ApiError(400, 'Project is not selected');

        const updated = await completeProjectNote(teamId, projectId, noteId);
        setNotes(prev => prev.map(note => note.id === noteId ? updated : note).sort(byNoteOrder));
        return updated;
    }, [projectId, teamId]);

    const reopenNote = useCallback(async (noteId: string) => {
        if (!teamId || !projectId) throw new ApiError(400, 'Project is not selected');

        const updated = await reopenProjectNote(teamId, projectId, noteId);
        setNotes(prev => prev.map(note => note.id === noteId ? updated : note).sort(byNoteOrder));
        return updated;
    }, [projectId, teamId]);

    const removeNote = useCallback(async (noteId: string) => {
        if (!teamId || !projectId) throw new ApiError(400, 'Project is not selected');

        await deleteProjectNote(teamId, projectId, noteId);
        setNotes(prev => prev.filter(note => note.id !== noteId));
    }, [projectId, teamId]);

    return {
        project,
        setProject,
        versions,
        setVersions,
        notes,
        setNotes,
        selectedVersion,
        selectedVersionId,
        setSelectedVersionId,
        loading,
        error,
        refresh,
        refreshNotes,
        updateProject,
        updateProjectPicture,
        removeProject,
        uploadVersion,
        removeVersion,
        addNote,
        updateNote,
        completeNote,
        reopenNote,
        removeNote,
    };
}
