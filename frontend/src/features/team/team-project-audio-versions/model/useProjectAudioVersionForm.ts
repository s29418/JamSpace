import React, { useRef, useState } from 'react';
import type { ProjectAudioVersion, UploadProjectAudioVersionRequest } from 'entities/team/model/types';
import { getErrorMessage } from 'entities/team/lib/teamProjectFormatters';

type UseProjectAudioVersionFormParams = {
    uploadVersion: (request: UploadProjectAudioVersionRequest) => Promise<ProjectAudioVersion>;
    removeVersion: (versionId: string) => Promise<void>;
};

export const useProjectAudioVersionForm = ({
    uploadVersion,
    removeVersion,
}: UseProjectAudioVersionFormParams) => {
    const fileInputRef = useRef<HTMLInputElement | null>(null);
    const [isFormOpen, setIsFormOpen] = useState(false);
    const [name, setName] = useState('');
    const [file, setFile] = useState<File | null>(null);
    const [error, setError] = useState<string | null>(null);
    const [saving, setSaving] = useState(false);
    const [deleting, setDeleting] = useState(false);
    const [versionToDelete, setVersionToDelete] = useState<ProjectAudioVersion | null>(null);

    const resetForm = () => {
        setIsFormOpen(false);
        setName('');
        setFile(null);
        setError(null);
        if (fileInputRef.current) fileInputRef.current.value = '';
    };

    const toggleForm = () => {
        setError(null);
        setIsFormOpen(current => !current);
    };

    const submit = async (event: React.FormEvent<HTMLFormElement>) => {
        event.preventDefault();
        setError(null);

        const trimmedName = name.trim();
        if (!trimmedName) {
            setError('Version name is required.');
            return;
        }

        if (!file) {
            setError('Audio file is required.');
            return;
        }

        try {
            setSaving(true);
            await uploadVersion({ name: trimmedName, file });
            resetForm();
        } catch (e) {
            setError(getErrorMessage(e, 'Failed to upload audio version.'));
        } finally {
            setSaving(false);
        }
    };

    const deleteSelectedVersion = async () => {
        if (!versionToDelete) return;

        try {
            setDeleting(true);
            await removeVersion(versionToDelete.id);
            setVersionToDelete(null);
        } catch (e) {
            setError(getErrorMessage(e, 'Failed to delete audio version.'));
            setVersionToDelete(null);
        } finally {
            setDeleting(false);
        }
    };

    return {
        fileInputRef,
        isFormOpen,
        name,
        file,
        error,
        saving,
        deleting,
        versionToDelete,
        setName,
        setFile,
        toggleForm,
        resetForm,
        submit,
        requestDeleteVersion: setVersionToDelete,
        cancelDeleteVersion: () => {
            if (!deleting) setVersionToDelete(null);
        },
        deleteSelectedVersion,
    };
};
